using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class PlayerShapePacket : Packet
    {
        //handle player shape
        public PlayerShapePacket(string name) : base(name) {}

        protected override void RunPacket(string data)
        { 
            Instance.GetMultiplayerHandler().MultiTetImage = Utils.TranslateShapeToImage(int.Parse("" + data[0]), false);
            base.RunPacket(data);
        }

        protected override void SendPacket(string data)
        {
            base.SendPacket(data);
        }
    }
}
