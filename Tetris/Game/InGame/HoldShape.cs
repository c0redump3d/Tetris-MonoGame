using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Player;
using Tetris.GUI.DebugMenu;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.InGame
{
    public class HoldShape
    {
        private bool disabled;
        private bool hasUsed;

        private int heldShape;
        private Rectangle hFour;

        private bool holdingShape;
        private Rectangle hOne;
        private Rectangle hThree;
        private Rectangle hTwo;
        private int hX = 34;
        private int hY = 50;
        private Texture2D tetImage;

        public HoldShape()
        {
            tetImage = Globals.BlockTexture[0];
        }

        public void ResetHold()
        {
            if (holdingShape)
            {
                hOne = new Rectangle(0, 0, 0, 0);
                hTwo = new Rectangle(0, 0, 0, 0);
                hThree = new Rectangle(0, 0, 0, 0);
                hFour = new Rectangle(0, 0, 0, 0);
                holdingShape = false;
                hasUsed = false;
                heldShape = 0;
            }
        }

        public void SetHoldShape(int shape)
        {
            if (hasUsed || disabled)
            {
                Sfx.Instance.PlaySoundEffect("holdfail");
                return;
            }

            if (holdingShape)
            {
                PlayerController.Instance.SetPlayerShape(heldShape, false);
                Sfx.Instance.PlaySoundEffect("hold");
                holdingShape = false;
            }
            else
            {
                PlayerController.Instance.SetPlayerShape(NextShape.Instance.GetNextShape(), true);
                Sfx.Instance.PlaySoundEffect("hold");
            }

            if (shape == 1)
            {
                hX = 34;
                hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 2)
            {
                hX = 34;
                hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 32, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 3)
            {
                hX = 34;
                hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX - 32, hY - 32, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 4)
            {
                hX = 27;
                hY = 40;
                hOne = new Rectangle(hX, hY, 24, 24); // player controlled rect
                hTwo = new Rectangle(hX + 24, hY + 0, 24, 24);
                hThree = new Rectangle(hX - 24, hY - 0, 24, 24);
                hFour = new Rectangle(hX + 48, hY - 0, 24, 24);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 5)
            {
                hX = 17;
                hY = 20;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 32, 32, 32);
                hThree = new Rectangle(hX - 0, hY - -32, 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 6)
            {
                hX = 34;
                hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + -32, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 7)
            {
                hX = 34;
                hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + -32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - -32, hY - 32, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape - 1];
            }

            heldShape = shape;
            holdingShape = true;
            hasUsed = true;
            NetworkManager.Instance.SendPacket(5);

            DebugConsole.Instance.AddMessage($"Holding block {shape}");
        }

        public void DrawHoldShape(SpriteBatch spriteBatch)
        {
            DrawBlocks(spriteBatch);
        }

        private void DrawBlocks(SpriteBatch spriteBatch)
        {
            if (tetImage == null)
                return;

            if (hasUsed)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    DepthStencilState.Default, RasterizerState.CullCounterClockwise, null,
                    Matrix.CreateTranslation(new Vector3(NetworkManager.Instance.Connected ? 110 : 310, 60, 0)));
                spriteBatch.Draw(Globals.BlockTexture[7], hOne, Color.White);
                spriteBatch.Draw(Globals.BlockTexture[7], hTwo, Color.White);
                spriteBatch.Draw(Globals.BlockTexture[7], hThree, Color.White);
                spriteBatch.Draw(Globals.BlockTexture[7], hFour, Color.White);
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    DepthStencilState.Default, RasterizerState.CullCounterClockwise, null,
                    Matrix.CreateTranslation(new Vector3(NetworkManager.Instance.Connected ? 110 : 310, 60, 0)));
                spriteBatch.Draw(tetImage, hOne, Color.White);
                spriteBatch.Draw(tetImage, hTwo, Color.White);
                spriteBatch.Draw(tetImage, hThree, Color.White);
                spriteBatch.Draw(tetImage, hFour, Color.White);
                spriteBatch.End();
            }
        }

        public void DisallowSwap()
        {
            disabled = true;
        }

        public void AllowSwap()
        {
            hasUsed = false;
            disabled = false;
        }
        
        private static HoldShape _instance;
        public static HoldShape Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new HoldShape();
                }

                return result;
            }
        }
    }
}