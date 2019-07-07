using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Resources.Loaders;
using Rhisis.Core.Structures.Game;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Helpers;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Leveling.EventArgs;

namespace Rhisis.World.Systems.Leveling
{
    /// <summary>
    /// Leveling system.
    /// </summary>
    [System(SystemType.Notifiable)]
    public sealed class LevelSystem : ISystem
    {
        private readonly ILogger<LevelSystem> _logger = DependencyContainer.Instance.Resolve<ILogger<LevelSystem>>();
        private readonly ExpTableLoader _expTableLoader = DependencyContainer.Instance.Resolve<ExpTableLoader>();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs args)
        {
            if (!(entity is IPlayerEntity player))
            {
                this._logger.LogError($"Cannot execute Level System. {entity.Object.Name} is not a player.");
                return;
            }

            if (!args.GetCheckArguments())
            {
                this._logger.LogError($"Cannot execute Level System action: {args.GetType()} due to invalid arguments.");
                return;
            }

            switch (args)
            {
                case ExperienceEventArgs e:
                    this.GiveExperience(player, e);
                    break;
                case ChangeJobEventArgs e:
                    this.ChangeJob(player, e);
                    break;
                default:
                    this._logger.LogWarning("Unknown level system action type: {0} for player {1}", args.GetType(), entity.Object.Name);
                    break;
            }
        }

        /// <summary>
        /// Give experience to a player.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="e">Experience event info.</param>
        private void GiveExperience(IPlayerEntity player, ExperienceEventArgs e)
        {
            int baseJobLevelLimit = (int)DefineJob.JobMax.MAX_JOB_LEVEL;
            int expertJobLevelLimit = (int)DefineJob.JobMax.MAX_EXPERT_LEVEL;

            if ((player.PlayerData.JobData.Type == DefineJob.JobType.JTYPE_BASE && player.Object.Level >= baseJobLevelLimit) ||
                (player.PlayerData.JobData.Type == DefineJob.JobType.JTYPE_EXPERT && player.Object.Level >= expertJobLevelLimit))
            {
                if (player.PlayerData.Experience != 0)
                {
                    player.PlayerData.Experience = 0;
                    WorldPacketFactory.SendPlayerExperience(player);
                }
                return;
            }

            if (player.PlayerData.JobData.Type == DefineJob.JobType.JTYPE_PRO && player.Object.Level > (int)DefineJob.JobMax.MAX_LEVEL)
            {
                player.PlayerData.Experience = 0;
                player.Object.Level = (int)DefineJob.JobMax.MAX_LEVEL;
                return;
            }

            long experience = this.CalculateExtraExperience(player, e.Experience);

            // TODO: experience to party

            if (this.GiveExperienceToPlayer(player, experience))
            {
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.HP, player.Health.Hp);
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.MP, player.Health.Mp);
                WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.FP, player.Health.Fp);
                WorldPacketFactory.SendPlayerSetLevel(player, player.Object.Level);
                WorldPacketFactory.SendPlayerStatsPoints(player);
            }

            WorldPacketFactory.SendPlayerExperience(player);
            // TODO: send packet to friends, messenger, guild, couple, party, etc...
        }

        /// <summary>
        /// Give experience to a player and returns a boolean value that indicates if the player has level up.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="experience">Experience to give.</param>
        /// <returns>True if the player has level up; false otherwise.</returns>
        private bool GiveExperienceToPlayer(IPlayerEntity player, long experience)
        {
            int nextLevel = player.Object.Level + 1;
            CharacterExpTableData nextLevelExpTable = GameResources.Instance.ExpTables.GetCharacterExp(nextLevel);
            player.PlayerData.Experience += experience;

            if (player.PlayerData.Experience >= nextLevelExpTable.Exp) // Level up
            {
                long remainingExp = player.PlayerData.Experience - nextLevelExpTable.Exp;

                this.ProcessLevelUp(player, (ushort)nextLevelExpTable.Gp);
                
                if (remainingExp > 0)
                    this.GiveExperienceToPlayer(player, remainingExp); // Multiple level up

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates extra experience with scrolls, events, skill bonus, etc...
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="experience">Current experience.</param>
        /// <returns>Base experience with extra experience.</returns>
        private long CalculateExtraExperience(IPlayerEntity player, long experience)
        {
            // TODO: add exp scrolls logic here

            return experience;
        }

        /// <summary>
        /// Process the level up logic and reward attribution.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="statPoints">Statistics points.</param>
        private void ProcessLevelUp(IPlayerEntity player, ushort statPoints)
        {
            player.Object.Level += 1;

            if (player.Object.Level != player.PlayerData.DeathLevel)
            {
                player.Statistics.SkillPoints += (ushort)(((player.Object.Level - 1) / 20) + 2);
                player.Statistics.StatPoints += statPoints;
            }

            player.PlayerData.Experience = 0;
            PlayerHelper.SetPoints(player, DefineAttributes.HP, PlayerHelper.GetMaxOriginHp(player));
            PlayerHelper.SetPoints(player, DefineAttributes.MP, PlayerHelper.GetMaxOriginMp(player));
            PlayerHelper.SetPoints(player, DefineAttributes.FP, PlayerHelper.GetMaxOriginFp(player));
        }

        /// <summary>
        /// Changes player's job.
        /// </summary>
        /// <param name="player">Player entity.</param>
        /// <param name="e">Change job event.</param>
        private void ChangeJob(IPlayerEntity player, ChangeJobEventArgs e)
        {
            if (player.PlayerData.JobData.Type == DefineJob.JobType.JTYPE_BASE && player.Object.Level >= (int)DefineJob.JobMax.MAX_JOB_LEVEL)
            {
                player.Object.Level = (int)DefineJob.JobMax.MAX_JOB_LEVEL;
            }
            else if (player.PlayerData.JobData.Type == DefineJob.JobType.JTYPE_EXPERT && player.Object.Level >= (int)DefineJob.JobMax.MAX_EXPERT_LEVEL)
            {
                player.Object.Level = (int)DefineJob.JobMax.MAX_EXPERT_LEVEL;
            }
            else
            {
                this._logger.LogWarning($"Cannot change job for player {player.Object.Name}.");
                return;
            }

            player.PlayerData.Job = (DefineJob.Job)e.JobId;
            PlayerHelper.SetPoints(player, DefineAttributes.HP, PlayerHelper.GetMaxHP(player));
            PlayerHelper.SetPoints(player, DefineAttributes.MP, PlayerHelper.GetMaxMP(player));
            PlayerHelper.SetPoints(player, DefineAttributes.FP, PlayerHelper.GetMaxFP(player));

            if (e.Restat)
            {
                int destLevel = player.Object.Level < player.PlayerData.DeathLevel ? player.PlayerData.DeathLevel : player.Object.Level;
                int statPoints = 0;
                for (int i = 1; i < destLevel; i++)
                {
                    statPoints += (int)this._expTableLoader.CharacterExpTable[i].Gp;
                    // TODO: check if master or hero
                }

                player.Statistics.StatPoints = (ushort)statPoints;
                player.Attributes.ResetAttribute(DefineAttributes.STR, 15);
                player.Attributes.ResetAttribute(DefineAttributes.STA, 15);
                player.Attributes.ResetAttribute(DefineAttributes.DEX, 15);
                player.Attributes.ResetAttribute(DefineAttributes.INT, 15);
                WorldPacketFactory.SendUpdateState(player);
                WorldPacketFactory.SendSpecialEffect(player, DefineSpecialEffects.XI_SYS_EXPAN01);
            }

            // TODO: set skills

            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.HP, PlayerHelper.GetPoints(player, DefineAttributes.HP));
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.MP, PlayerHelper.GetPoints(player, DefineAttributes.MP));
            WorldPacketFactory.SendUpdateAttributes(player, DefineAttributes.FP, PlayerHelper.GetPoints(player, DefineAttributes.FP));
            WorldPacketFactory.SendPlayerSetLevel(player, player.Object.Level);
            WorldPacketFactory.SendPlayerExperience(player);
            WorldPacketFactory.SendPlayerJobSkills(player);
            WorldPacketFactory.SendSpecialEffect(player, DefineSpecialEffects.XI_GEN_LEVEL_UP01);
        }
    }
}
