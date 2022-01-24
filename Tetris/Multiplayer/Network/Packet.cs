using System;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Multiplayer.Network.Packets;

namespace Tetris.Multiplayer.Network
{
    public class Packet
    {
        private readonly List<Packet> packetList = new(0);
        protected int PacketID { get; } // Packet ID is the value used to find a specific packet in game, for example ID 2 is BoardPacket
        protected NetDataWriter dataWriter;

        public Packet()
        {
            //Add all used packets to packetList.
            CreatePacket(new PlayerConnectPacket(1));
            CreatePacket(new BoardPacket(2));
            CreatePacket(new PlayerPositionPacket(3));
            CreatePacket(new SendGarbagePacket(4));
            CreatePacket(new ShapeRotationPacket(5));
            CreatePacket(new StartGamePacket(6));
            CreatePacket(new EndGamePacket(7));
            CreatePacket(new PausePacket(8));
        }

        protected Packet(int id)
        {
            //set the packetName
            this.PacketID = id;
        }

        private void CreatePacket(Packet packet)
        {
            packetList.Add(packet);
        }

        /// <summary>
        /// What to run after packet has been received.
        /// </summary>
        /// <param name="packetReader">Information to be processed with the packet</param>
        protected virtual void RunPacket(NetPacketReader packetReader) {}

        /// <summary>
        /// Sends our packet to the client/server to be run.
        /// </summary>
        protected virtual void SendPacket()
        {
            NetworkManager.Instance.SendInformation(dataWriter);
        }

        /// <summary>
        /// Allows a packet to be sent to the client/server by giving its packetName value. This can also include data if provided (see parameter data).
        /// </summary>
        /// <param name="pName">packetName(ex: dis)</param>
        /// <param name="reader">Included information with the packet(ex: placedRect pos/colors)</param>
        public void SendPacketFromID(int packetID)
        {
            foreach (Packet packet in packetList)
            {
                if (packet.PacketID == packetID) // if the packetName equals the provided pName, then send that packet
                {
                    packet.SendPacket();
                }
            }
        }
        
        /// <summary>
        /// Executes the packet provided (Note: can include data).
        /// </summary>
        /// <param name="pName">packetName</param>
        /// <param name="data"></param>
        public void RunPacketFromID(int packetID, NetPacketReader reader)
        {
            foreach (Packet packet in packetList)
            {
                if (packet.PacketID == packetID) // if the packetName equals the provided pName, then send that packet
                {
                    packet.RunPacket(reader);
                }
            }
        }

        /// <summary>
        /// Returns the packet when given its name(packetName).
        /// </summary>
        /// <param name="packetID">PacketID</param>
        /// <returns>Packet object from PacketID</returns>
        public Packet GetPacketFromID(int packetID)
        {
            foreach (var packet in packetList)
            {
                if (packet.PacketID == packetID) // if the packetName equals the provided pName, then return the packet
                {
                    return packet;
                }
            }

            return null;
        }
    }
}