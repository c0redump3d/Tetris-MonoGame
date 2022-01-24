using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game;
using Tetris.Game.Managers;

namespace Tetris.Multiplayer.Network.Packets
{
    public class PausePacket : Packet
    {
        public PausePacket(int id) : base(id) {}

        protected override void RunPacket(NetPacketReader packetReader)
        {
            InGameManager.Instance.PauseGame();
            base.RunPacket(packetReader);
        }

        protected override void SendPacket()
        {
            dataWriter = new NetDataWriter();
            dataWriter.Put(PacketID);
            base.SendPacket();
        }
    }
}