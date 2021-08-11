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

            Instance.GetSound().PlayBackground();
            Instance.GetGame().StartCountdown();
            Instance.GetGame().CurrentMode = 0;
            Instance.GetPlayer().PlyY = 0;
            Instance.GetMultiplayerHandler().PlacedRect = new Rectangle[0];
            base.RunPacket();
        }

        protected override void SendPacket()
        {
            base.SendPacket();
        }
    }
}
