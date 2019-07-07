using NLog;
using Rhisis.Core.Data;
using Rhisis.Core.Structures.Game;
using Rhisis.Core.Structures.Game.Dialogs;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Core.Systems;
using Rhisis.World.Game.Entities;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Leveling;
using Rhisis.World.Systems.Leveling.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.NpcDialog
{
    [System(SystemType.Notifiable)]
    public class NpcDialogSystem : ISystem
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public WorldEntityType Type => WorldEntityType.Player;

        /// <inheritdoc />
        public void Execute(IEntity entity, SystemEventArgs e)
        {
            if (!(entity is IPlayerEntity player) || !(e is NpcDialogOpenEventArgs dialogEvent))
            {
                Logger.Error("DialogSystem: Invalid event arguments.");
                return;
            }

            if (!dialogEvent.GetCheckArguments())
            {
                Logger.Error("DialogSystem: Invalid event action arguments.");
                return;
            }

            this.OpenDialog(player, dialogEvent);
        }

        /// <summary>
        /// Open a NPC dialog box.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="e"></param>
        private void OpenDialog(IPlayerEntity player, NpcDialogOpenEventArgs e)
        {
            var npcEntity = player.Object.CurrentMap?.FindEntity<INpcEntity>(e.NpcObjectId);

            if (npcEntity == null)
            {
                Logger.Error("DialogSystem: Cannot find NPC with id: {0}", e.NpcObjectId);
                return;
            }

            // Temporary
            if (this.ProcessJobChange(player, npcEntity))
                return;

            if (!npcEntity.Data.HasDialog)
            {
                Logger.Error("DialogSystem: NPC '{0}' doesn't have a dialog.", npcEntity.Object.Name);
                return;
            }

            IEnumerable<string> dialogTexts = npcEntity.Data.Dialog.IntroText;

            if (!string.IsNullOrEmpty(e.DialogKey))
            {
                if (e.DialogKey == "BYE")
                {
                    WorldPacketFactory.SendChatTo(npcEntity, player, npcEntity.Data.Dialog.ByeText);
                    WorldPacketFactory.SendCloseDialog(player);
                    return;
                }
                else
                {
                    DialogLink dialogLink = npcEntity.Data.Dialog.Links?.FirstOrDefault(x => x.Id == e.DialogKey);

                    if (dialogLink == null)
                    {
                        Logger.Error("DialogSystem: Cannot find dialog key: '{0}' for NPC '{1}'", e.DialogKey, npcEntity.Object.Name);
                        return;
                    }

                    dialogTexts = dialogLink.Texts;
                }
            }

            WorldPacketFactory.SendDialog(player, dialogTexts, npcEntity.Data.Dialog.Links);
        }

        private bool ProcessJobChange(IPlayerEntity player, INpcEntity npc)
        {
            if (player.PlayerData.JobData.Type != DefineJob.JobType.JTYPE_BASE)
                return false;

            switch (npc.Object.Name)
            {
                case "MaFl_Mustang":
                    player.NotifySystem<LevelSystem>(new ChangeJobEventArgs((int)DefineJob.Job.JOB_MERCENARY, true));
                    WorldPacketFactory.SendDialog(player, new[] { "You are now a mercenary!" }, null);
                    return true;
                case "MaFl_Elic":
                    player.NotifySystem<LevelSystem>(new ChangeJobEventArgs((int)DefineJob.Job.JOB_ASSIST, true));
                    WorldPacketFactory.SendDialog(player, new[] { "You are now an assist!" }, null);
                    return true;
                case "MaSa_Wingyei":
                    player.NotifySystem<LevelSystem>(new ChangeJobEventArgs((int)DefineJob.Job.JOB_MAGICIAN, true));
                    WorldPacketFactory.SendDialog(player, new[] { "You are now a magician!" }, null);
                    return true;
                case "MaDa_Hent":
                    player.NotifySystem<LevelSystem>(new ChangeJobEventArgs((int)DefineJob.Job.JOB_ACROBAT, true));
                    WorldPacketFactory.SendDialog(player, new[] { "You are now an acrobat!" }, null);
                    return true;
            }

            return false;
        }
    }
}