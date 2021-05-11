using System;
using System.Collections.Generic;
using Tetris.Multiplayer.Network.Packets;
using Tetris.Other;

namespace Tetris.Multiplayer.Network
{
    /// <summary>
    /// Used to create a class to understand, receive, and run data from a client/server.
    /// </summary>
    public class Packet
    {
        private readonly List<Packet> packetList = new(0);
        private string PacketName { get; } // Packet name is the identifier for the server and client to understand what it should do with the information received (ex: plc = PlacedRectPacket)

        public Packet()
        {
            //Add all used packets to packetList.
            CreatePacket(new EndGamePacket("end"));
            CreatePacket(new PlacedRectPacket("plc"));
            CreatePacket(new PlayerConnectedPacket("con"));
            CreatePacket(new PlayerDisconnectedPacket("dis"));
            CreatePacket(new PlayerPositionPacket("pos"));
            CreatePacket(new PlayerShapePacket("shp"));
            CreatePacket(new SendBlockPacket("sdb"));
            CreatePacket(new StartGamePacket("str"));
        }

        protected Packet(string name)
        {
            //set the packetName
            this.PacketName = name;
        }

        private void CreatePacket(Packet packet)
        {
            packetList.Add(packet);
        }

        /// <summary>
        /// What to run after packet has been received.
        /// </summary>
        protected virtual void RunPacket() {}

        /// <summary>
        /// What to run after packet has been received (Takes string for data).
        /// </summary>
        /// <param name="data">Information to be processed with the packet</param>
        protected virtual void RunPacket(string data) {}

        /// <summary>
        /// Sends our packet(packetName) to the client/server to be run.
        /// </summary>
        protected virtual void SendPacket()
        {
            //Used to send a packet with no data to the client/server (ex: PlayerDisconnectPacket = "dis;").
            if(Server.ConnectionEstablished())
                Server.SendData($"{this.PacketName};");
            else
                Client.SendData($"{this.PacketName};");
        }

        /// <summary>
        /// Sends our packet(packetName) along with included data to the client/server to be run.
        /// </summary>
        /// <param name="data">Information to send to the client/server</param>
        protected virtual void SendPacket(string data)
        {
            //If our packet contains data, include that when sending to client/server.
            if(Server.ConnectionEstablished())
                Server.SendData($"{this.PacketName} {data};");
            else
                Client.SendData($"{this.PacketName} {data};");
        }

        /// <summary>
        /// Allows a packet to be sent to the client/server by giving its packetName value. This can also include data if provided (see parameter data).
        /// </summary>
        /// <param name="pName">packetName(ex: dis)</param>
        /// <param name="data">Included information with the packet(ex: placedRect pos/colors)</param>
        public void SendPacketFromName(string pName, string data = "")
        {
            bool containsData = data != "";
            
            foreach (Packet packet in packetList)
            {
                if (packet.PacketName.Equals(pName, StringComparison.OrdinalIgnoreCase)) // if the packetName equals the provided pName, then send that packet
                {
                    if(!containsData)
                        packet.SendPacket();
                    else
                        packet.SendPacket($"{data}");
                }
            }
        }
        
        /// <summary>
        /// Executes the packet provided (Note: can include data).
        /// </summary>
        /// <param name="pName">packetName</param>
        /// <param name="data"></param>
        public void RunPacketFromName(string pName, string data = "")
        {
            bool containsData = data != "";
            
            foreach (Packet packet in packetList)
            {
                if (packet.PacketName.Equals(pName, StringComparison.OrdinalIgnoreCase)) // if the packetName equals the provided pName, then run that packet
                {
                    if(!containsData)
                        packet.RunPacket();
                    else
                        packet.RunPacket(data);
                }
            }
        }

        /// <summary>
        /// Returns the packet when given its name(packetName).
        /// </summary>
        /// <param name="pName">packetName</param>
        /// <returns>Packet object from packetName</returns>
        public Packet GetPacketFromName(string pName)
        {
            foreach (var packet in packetList)
            {
                if (packet.PacketName.Equals(pName, StringComparison.OrdinalIgnoreCase)) // if the packetName equals the provided pName, then return the packet
                {
                    return packet;
                }
            }

            return null;
        }

        protected bool InMultiplayer()
        {
            return Instance.InMultiplayer;
        }
        
        protected bool IsServer()
        {
            return Server.ConnectionEstablished();
        }

    }
}
