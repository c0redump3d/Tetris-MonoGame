using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Other;

namespace Tetris.Multiplayer.Network.Packets
{
    class PlacedRectPacket : Packet
    {

        public PlacedRectPacket(string name) : base(name) {}

        protected override void RunPacket(string data)
        {
            Instance.GetMultiplayerHandler().PlacedRect = new Rectangle[0];
            Instance.GetMultiplayerHandler().StoredImage = new Texture2D[0];

            //InstanceManager.GetMainForm().MultiShape = int.Parse(_data.Split(' ')[1]); Not needed anymore since we parse it under this to set the image.
            //set second players image to the given shape at index 0
            Instance.GetMultiplayerHandler().MultiTetImage = Utils.TranslateShapeToImage(int.Parse("" + data[0]), false);

            //remove player shape from data (# )
            string[] placedPos = data.Remove(0,2).Split(',');

            //loop through the given data and translate string data to placedRect positions and colors.
            List<Rectangle> createBlock = Instance.GetMultiplayerHandler().PlacedRect.ToList();
            List<Texture2D> addTexture = Instance.GetMultiplayerHandler().StoredImage.ToList();
            for (int i = 0; i < placedPos.Length - 1; i++)
            {
                string[] splitPos = placedPos[i].Split(':');
                createBlock.Add(new Rectangle(int.Parse(splitPos[0]), int.Parse(splitPos[1]), 32, 32));
                addTexture.Add(Utils.TranslateNameToTexture(splitPos[2]));
            }
            Instance.GetMultiplayerHandler().PlacedRect = createBlock.ToArray();
            Instance.GetMultiplayerHandler().StoredImage = addTexture.ToArray();

            base.RunPacket();
        }

        protected override void SendPacket()
        {
            if (!InMultiplayer())
                return;
            string dataToSend = "";
            string converted = "";
            
            //Loop through placedRect and translate positions and color into a string 
            for (int l = 0; l < Instance.GetPlayer().PlacedRect.Length; l++)
            {
                char block;
                if (Instance.GetPlayer().StoredImage[l] != null)
                    block = Utils.TranslateToShapeChar(Instance.GetPlayer().StoredImage[l].Name);
                else
                    block = 'x';
                converted += Instance.GetPlayer().PlacedRect[l].X + ":" +
                             Instance.GetPlayer().PlacedRect[l].Y + ":" +
                             block + ",";
            }

            //send converted placedRect positions to server
            dataToSend = $@"{Instance.GetRotate().GetCurShape()}/{converted}";
            base.SendPacket(dataToSend); // Information is ready, now send to client/server
        }

    }
}
