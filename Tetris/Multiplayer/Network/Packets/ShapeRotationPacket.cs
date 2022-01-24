using LiteNetLib;
using LiteNetLib.Utils;
using Tetris.Game.Player;

namespace Tetris.Multiplayer.Network.Packets
{
    public class ShapeRotationPacket : Packet
    {
        public ShapeRotationPacket(int id) : base(id){}
        
        protected override void RunPacket(NetPacketReader packetReader)
        {
            int shape = packetReader.GetInt();
            int rotation = packetReader.GetInt();
            
            PlayerMP.Instance.SetShape(shape, rotation);
            base.RunPacket(packetReader);
        }

        protected override void SendPacket()
        {
            dataWriter = new NetDataWriter();
            dataWriter.Put(PacketID);
            dataWriter.Put(Rotate.Instance.GetCurShape());
            dataWriter.Put(Rotate.Instance.GetCurAngle());
            base.SendPacket();
        }
    }
}