using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class SendBlockPacket : Packet
    {
        public SendBlockPacket(string name) : base(name) {}

        protected override void RunPacket(string data)
        {
            Instance.GetPlayer().RandomBlock(int.Parse($"{data[0]}"));
            base.RunPacket(data);
        }

        protected override void SendPacket(string data)
        {
            base.SendPacket(data);
        }
    }
}
