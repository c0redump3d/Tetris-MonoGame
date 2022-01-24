using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game.Player;

namespace Tetris.Multiplayer.Network.Packets
{
    public class PlayerPositionPacket : Packet
    {
        public PlayerPositionPacket(int id) : base(id){}
        
        protected override void RunPacket(NetPacketReader packetReader)
        {
            int[] posArr = packetReader.GetIntArray();

            PlayerMP.Instance.PosX = posArr[0];
            PlayerMP.Instance.PosY = posArr[1];
            base.RunPacket(packetReader);
        }

        protected override void SendPacket()
        {
            int[] posArr = {PlayerController.Instance.PlyX, PlayerController.Instance.PlyY};
            
            dataWriter = new NetDataWriter();
            dataWriter.Put(PacketID);
            dataWriter.PutArray(posArr);
            base.SendPacket();
        }
    }
}