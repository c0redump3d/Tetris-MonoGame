using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Other;

namespace Tetris.Main.Player
{
    public class HoldShape
    {
        private Rectangle hOne;
        private Rectangle hTwo;
        private Rectangle hThree;
        private Rectangle hFour;
        private Texture2D tetImage;
        private int hX = 34;
        private int hY = 50;

        private bool holdingShape;
        private bool hasUsed;
        private bool disabled;

        private int heldShape;

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
                Instance.GetSound().PlaySoundEffect("holdfail");
                return;
            }

            if (holdingShape)
            {
                Instance.GetPlayer().SetPlayerShape(heldShape, false);
                Instance.GetSound().PlaySoundEffect("hold");
                holdingShape = false;
            }
            else
            {
                Instance.GetPlayer().SetPlayerShape(Instance.GetNextShape().GetNextShape(), true);
                Instance.GetSound().PlaySoundEffect("hold");
            }

            if (shape == 1)
            {
                hX = 34; hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 2)
            {
                hX = 34; hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 32, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 3)
            {
                hX = 34; hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX - 32, hY - 32, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 4)
            {
                hX = 27; hY = 40;
                hOne = new Rectangle(hX, hY, 24, 24); // player controlled rect
                hTwo = new Rectangle(hX + 24, hY + 0, 24, 24);
                hThree = new Rectangle(hX - 24, hY - 0, 24, 24);
                hFour = new Rectangle(hX + 48, hY - 0, 24, 24);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 5)
            {
                hX = 17; hY = 20;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + 32, 32, 32);
                hThree = new Rectangle(hX - 0, hY - (-32), 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 6)
            {
                hX = 34; hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + 32, hY + -32, 32, 32);
                hThree = new Rectangle(hX - 32, hY - 0, 32, 32);
                hFour = new Rectangle(hX + 32, hY - 0, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }
            else if (shape == 7)
            {
                hX = 34; hY = 50;
                hOne = new Rectangle(hX, hY, 32, 32); // player controlled rect
                hTwo = new Rectangle(hX + -32, hY + 0, 32, 32);
                hThree = new Rectangle(hX - (-32), hY - 32, 32, 32);
                hFour = new Rectangle(hX + 0, hY - 32, 32, 32);
                tetImage = Globals.BlockTexture[shape-1];
            }

            heldShape = shape;
            holdingShape = true;
            hasUsed = true;

            Instance.GetPacket().SendPacketFromName("shp",$"{Instance.GetRotate().GetCurShape()}");

            Instance.GetGuiDebug().DebugMessage($"Holding block {shape}");
        }

        public void DrawHoldShape(SpriteBatch _spriteBatch)
        {
            DrawBlocks(_spriteBatch);
        }

        private void DrawBlocks(SpriteBatch _spriteBatch)
        {
            if (tetImage == null)
                return;
            
            if (hasUsed)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.CreateTranslation(new Vector3(66,53, 0)));
                _spriteBatch.Draw(Globals.BlockTexture[7], hOne, Color.White);
                _spriteBatch.Draw(Globals.BlockTexture[7], hTwo, Color.White);
                _spriteBatch.Draw(Globals.BlockTexture[7], hThree, Color.White);
                _spriteBatch.Draw(Globals.BlockTexture[7], hFour, Color.White);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.CreateTranslation(new Vector3(66,53, 0)));
                _spriteBatch.Draw(tetImage, hOne, Color.White);
                _spriteBatch.Draw(tetImage, hTwo, Color.White);
                _spriteBatch.Draw(tetImage, hThree, Color.White);
                _spriteBatch.Draw(tetImage, hFour, Color.White);
                _spriteBatch.End();
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

    }
}
