﻿using Rhisis.Core.Data;
using Rhisis.Core.Structures;
using Rhisis.Network;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Core;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;

namespace Rhisis.World.Packets
{
    public static partial class WorldPacketFactory
    {
        public static void SendSpeedFactor(IEntity entity, float speedFactor)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SET_SPEED_FACTOR);
                packet.Write(speedFactor);

                SendToVisible(packet, entity);
            }
        }

        public static void SendMotion(IEntity entity, ObjectMessageType objectMessage)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOTION);
                packet.Write((int)objectMessage);

                SendToVisible(packet, entity, sendToPlayer: true);
            }
        }

        public static void SendMoverMoved(IEntity entity, Vector3 beginPosition, Vector3 destinationPosition,
            float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption,
            long tickCount)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERMOVED);
                packet.Write(beginPosition.X);
                packet.Write(beginPosition.Y);
                packet.Write(beginPosition.Z);
                packet.Write(destinationPosition.X);
                packet.Write(destinationPosition.Y);
                packet.Write(destinationPosition.Z);
                packet.Write(angle);
                packet.Write(state);
                packet.Write(stateFlag);
                packet.Write(motion);
                packet.Write(motionEx);
                packet.Write(loop);
                packet.Write(motionOption);
                packet.Write(tickCount);

                SendToVisible(packet, entity, sendToPlayer: false);
            }
        }

        public static void SendMoverBehavior(IEntity entity, Vector3 beginPosition, Vector3 destinationPosition,
            float angle, uint state, uint stateFlag, uint motion, int motionEx, int loop, uint motionOption,
            long tickCount)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.MOVERBEHAVIOR);
                packet.Write(beginPosition.X);
                packet.Write(beginPosition.Y);
                packet.Write(beginPosition.Z);
                packet.Write(destinationPosition.X);
                packet.Write(destinationPosition.Y);
                packet.Write(destinationPosition.Z);
                packet.Write(angle);
                packet.Write(state);
                packet.Write(stateFlag);
                packet.Write(motion);
                packet.Write(motionEx);
                packet.Write(loop);
                packet.Write(motionOption);
                packet.Write(tickCount);

                SendToVisible(packet, entity, sendToPlayer: false);
            }
        }

        public static void SendDestinationPosition(IMovableEntity movableEntity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(movableEntity.Id, SnapshotType.DESTPOS);
                packet.Write(movableEntity.Moves.DestinationPosition.X);
                packet.Write(movableEntity.Moves.DestinationPosition.Y);
                packet.Write(movableEntity.Moves.DestinationPosition.Z);
                packet.Write<byte>(1);

                SendToVisible(packet, movableEntity);
            }
        }

        public static void SendMoverPosition(IEntity entity)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.SETPOS);
                packet.Write(entity.Object.Position.X);
                packet.Write(entity.Object.Position.Y);
                packet.Write(entity.Object.Position.Z);

                SendToVisible(packet, entity, sendToPlayer: true);
            }
        }

        public static void SendDestinationAngle(IEntity entity, bool left)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.DESTANGLE);
                packet.Write(entity.Object.Angle);
                packet.Write(left);

                SendToVisible(packet, entity);
            }
        }

        public static void SendStateMode(IEntity entity, StateModeBaseMotion flags, Item item = null)
        {
            using (var packet = new FFPacket())
            {
                packet.StartNewMergedPacket(entity.Id, SnapshotType.STATEMODE);
                packet.Write((int)entity.Object.StateMode);
                packet.Write((byte)flags);

                if (flags == StateModeBaseMotion.BASEMOTION_ON && item != null)
                    packet.Write(item.Id);

                SendToVisible(packet, entity, sendToPlayer: true);
            }
        }
    }
}
