using Microsoft.Xna.Framework;
using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class StartGamePacket : Packet
    {
        //Server only packet
        public StartGamePacket(string name) : base(name) {}

        protected override void RunPacket()
        {
            if (IsServer())
                return;
            Instance.GetGame().StartCountdown();
            Instance.GetPlayer().PlyY = 0;
            Instance.GetMultiplayerHandler().MultiPlacedRect = new Rectangle[0];
            base.RunPacket();
        }

        protected override void SendPacket()
        {
            base.SendPacket();
        }
    }
}
