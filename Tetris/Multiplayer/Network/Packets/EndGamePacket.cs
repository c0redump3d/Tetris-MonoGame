using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    public class EndGamePacket : Packet
    {

        public EndGamePacket(string name) : base(name) {}

        protected override void RunPacket()
        {
            if (Instance.GetGame().Sender)
                return;
            
            Instance.GetPlayer().PlacedRect.Add(new Rectangle(999, Globals.TopOut, 32, 32), Globals.BlockTexture[7]);
            Instance.GetGame().Winner = true;
            Instance.GetMultiplayerHandler().MultiPlacedRect = new Rectangle[0];
            Instance.GetMultiplayerHandler().StoredImage = new Texture2D[0];
            base.RunPacket();
        }

        protected override void SendPacket()
        {
            if (!InMultiplayer())
                return;

            Instance.GetMultiplayerHandler().MultiPlacedRect = new Rectangle[0];
            Instance.GetMultiplayerHandler().StoredImage = new Texture2D[0];
            Instance.GetGame().Sender = true;
            //Packet parent class will take care of the rest.
            base.SendPacket();
        }

    }
}
