using Rhisis.Core.Data;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        /// <summary>
        /// Sends player revival packet.
        /// </summary>
        /// <param name="entity"></param>
        public static void SendPlayerRevival(IEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.REVIVAL_TO_LODESTAR);

                SendToVisible(packet, entity, sendToPlayer: true);
            }
        }

        public static void SendReplaceObject(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.REPLACE);
                packet.Write(player.Object.MapId);
                packet.Write(player.Object.Position.X);
                packet.Write(player.Object.Position.Y);
                packet.Write(player.Object.Position.Z);

                player.Connection.Send(packet);
            }
        }

        /// <summary>
        /// Send player update level.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="level"></param>
        public static void SendPlayerSetLevel(IPlayerEntity player, int level)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETLEVEL);
                packet.Write((short)level);

                SendToVisible(packet, player);
            }
        }

        /// <summary>
        /// Send Player experience.
        /// </summary>
        /// <param name="player"></param>
        public static void SendPlayerExperience(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SETEXPERIENCE);
                packet.Write(player.PlayerData.Experience);
                packet.Write((short)player.Object.Level);
                packet.Write(0); // TODO: total skill points
                packet.Write((int)player.Statistics.SkillPoints);
                packet.Write(long.MaxValue); // death exp
                packet.Write((short)player.PlayerData.DeathLevel);

                player.Connection.Send(packet);
            }
        }

        public static void SendPlayerJobSkills(IPlayerEntity player)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(player.Id, SnapshotType.SET_JOB_SKILL);
                packet.Write(player.PlayerData.JobId);

                for (int i = 0; i < (int)DefineJob.JobMax.MAX_SKILLS; ++i)
                {
                    packet.Write(-1); // skill id
                    packet.Write(0); // skill level
                }

                for (int i = 0; i < (int)DefineJob.JobMax.MAX_JOB; i++)
                    packet.Write(int.MaxValue);

                SendToVisible(packet, player, sendToPlayer: true);
            }
        }
    }
}
