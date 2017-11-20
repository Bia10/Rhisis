﻿using Ether.Network;
using Ether.Network.Packets;
using Rhisis.Core.IO;
using Rhisis.Core.Helpers;
using Rhisis.Core.Network;
using Rhisis.Core.Network.Packets;
using System.Collections.Generic;
using Rhisis.Core.Exceptions;
using Rhisis.World.Core.Entities;
using Rhisis.World.Core.Components;
using Rhisis.World.Game;
using Rhisis.Database;

namespace Rhisis.World
{
    public sealed class WorldClient : NetConnection
    {
        private readonly uint _sessionId;
        
        /// <summary>
        /// Gets or sets the player entity.
        /// </summary>
        public IEntity Player { get; set; }

        /// <summary>
        /// Creates a new <see cref="WorldClient"/> instance.
        /// </summary>
        public WorldClient()
        {
            this._sessionId = RandomHelper.GenerateSessionKey();
        }

        /// <summary>
        /// Initialize the client and send welcome packet.
        /// </summary>
        public void InitializeClient()
        {
            CommonPacketFactory.SendWelcome(this, this._sessionId);
        }

        /// <summary>
        /// Handles incoming messages.
        /// </summary>
        /// <param name="packet">Incoming packet</param>
        public override void HandleMessage(NetPacketBase packet)
        {
            var pak = packet as FFPacket;
            var packetHeader = new PacketHeader(pak);

            if (!FFPacket.VerifyPacketHeader(packetHeader, (int)this._sessionId))
            {
                Logger.Warning("Invalid header for packet: {0}", packetHeader.Header);
                return;
            }

            packet.Read<uint>(); // DPID: Always 0xFFFFFFFF
            var packetHeaderNumber = packet.Read<uint>();

            try
            {
                PacketHandler<WorldClient>.Invoke(this, pak, (PacketType)packetHeaderNumber);
            }
            catch (KeyNotFoundException)
            {
                FFPacket.UnknowPacket<PacketType>(packetHeaderNumber, 2);
            }
            catch (RhisisPacketException packetException)
            {
                Logger.Error(packetException.Message);
#if DEBUG
                Logger.Debug("STACK TRACE");
                Logger.Debug(packetException.InnerException?.StackTrace);
#endif
            }
        }

        /// <summary>
        /// Save the entity to the database.
        /// </summary>
        private void Save()
        {
            var objectComponent = this.Player.GetComponent<ObjectComponent>();
            var humanComponent = this.Player.GetComponent<HumanComponent>();
            var playerComponent = this.Player.GetComponent<PlayerComponent>();

            objectComponent.Spawned = false;
            
            using (var db = DatabaseService.GetContext())
            {
                var character = db.Characters.Get(playerComponent.Id);

                if (character != null)
                {
                    character.PosX = objectComponent.Position.X;
                    character.PosY = objectComponent.Position.Y;
                    character.PosZ = objectComponent.Position.Z;
                    character.Angle = objectComponent.Angle;
                    character.MapId = objectComponent.MapId;
                    character.Gender = humanComponent.Gender;
                    character.HairColor = humanComponent.HairColor;
                    character.HairId = humanComponent.HairId;
                    character.FaceId = humanComponent.FaceId;
                    character.SkinSetId = humanComponent.SkinSetId;

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Disposes the <see cref="WorldClient"/> resources.
        /// </summary>
        public override void Dispose()
        {
            var entityObjectComponent = this.Player?.GetComponent<ObjectComponent>();

            if (entityObjectComponent != null && WorldServer.Maps.TryGetValue(entityObjectComponent.MapId, out Map currentMap))
                currentMap.Context.DeleteEntity(this.Player);

            this.Save();

            base.Dispose();
        }
    }
}
