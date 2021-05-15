using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.GameDebug;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.Multiplayer
{
    public class MultiplayerHandler
    {
        //TODO: Sometimes player color is not correct, also center text
        private Rectangle mOne;
        private Rectangle mTwo;
        private Rectangle mThree;
        private Rectangle mFour;
        public Rectangle[] PlacedRect;
        public Texture2D[] StoredImage;
        public Texture2D MultiTetImage = Globals.BlockTexture[7];
        public int MultiX = 0;
        public int MultiY = 0;
        public int Mr1 = 0;
        public int Mr2 = 0;
        public int Ml1 = 0;
        public int Ml2 = 0;
        public int Mt1 = 0;
        public int Mt2 = 0;
        private SpriteFont font;
        private string playerText = "Not connected to any server.";

        public void ShowMultiplayer()
        {
            font = Globals.hoog_12;
            PlacedRect = new Rectangle[0];
            StoredImage = new Texture2D[0];
            Globals.CurrentGuiImage = Globals.GuiImage[1];
            Globals.ScreenWidth = 1172;
            Globals.ScreenHeight = 694;
            Globals.ResizedWindow = true;
        }

        public void HideMultiplayer()
        {
            PlacedRect = new Rectangle[0];
            StoredImage = new Texture2D[0];
            Globals.CurrentGuiImage = Globals.GuiImage[0];
            Globals.ScreenWidth = 789;
            Globals.ScreenHeight = 694;
            Globals.ResizedWindow = true;
        }

        /// <summary>
        /// This function is used to check if any placed tetris blocks reaches the top of the screen.
        /// If it does, it will end the game.
        /// </summary>
        private void HitTop()
        {
            if (Instance.GetGame().ScreenShake)
                return;
            for (int i = PlacedRect.Length - 1; i > 1; i--)
            {
                for (int f = 0; f < 4; f++)
                {
                    if (PlacedRect[i].Y == Globals.TopOut) // if placed rectangle reaches top of board, end game.
                    {
                        mOne = new();
                        mTwo = new();
                        mThree = new();
                        mFour = new();
                        Instance.GetPlayer().PlacedRect.Add(new Rectangle(999, Globals.TopOut,32,32), Globals.BlockTexture[7]);
                        return;
                    }
                }
            }
        }
        
        public void Update(GameTime gameTime)
        {
            HitTop();
            
            if (!Instance.IsPlayerConnected() && Instance.GetGame().Stopped)
            {
                if (Server.ServerRunning())
                    playerText = "Waiting for player to connect.";
                else
                    playerText = "Not connected to any server.";
            }

            if (Instance.IsPlayerConnected() && Instance.GetGame().Stopped)
            {
                if (Client.IsConnected())
                    playerText = "Connected! Waiting for host.";
                else
                    playerText = "Connected! Press start game.";
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            try
            {
                for (int i = PlacedRect.Length - 1; i > 0; i--)
                    spriteBatch.Draw(StoredImage[i], PlacedRect[i], Color.White);

                if (Instance.GetGame().Stopped)
                {
                    spriteBatch.DrawString(font, playerText, new Vector2(30, spriteBatch.GraphicsDevice.Viewport.Height / 2), Color.White);
                    return;
                }

                mOne = new Rectangle(MultiX, MultiY, 32, 32); // player controlled rect
                mTwo = new Rectangle(MultiX + Mr1, MultiY + Mr2, 32, 32);
                mThree = new Rectangle(MultiX - Ml1, MultiY - Ml2, 32, 32);
                mFour = new Rectangle(MultiX + Mt2, MultiY - Mt1, 32, 32);
                //draw player rectangles
                spriteBatch.Draw(MultiTetImage, mOne, Color.White);
                spriteBatch.Draw(MultiTetImage, mTwo, Color.White);
                spriteBatch.Draw(MultiTetImage, mThree, Color.White);
                spriteBatch.Draw(MultiTetImage, mFour, Color.White);
            }
            catch (Exception ex)
            {
                Debug.DebugMessage("SecondPlayerForm Error! Exception: " + ex.Message, 1, true);
            }
        }
    }
}