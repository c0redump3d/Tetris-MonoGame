using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.GUI.DebugMenu;
using Tetris.Multiplayer.Network;
using Tetris.Util;

namespace Tetris.Game.InGame
{
    public class NextShape
    {
        private readonly List<int> defaultOrder = new() {1, 2, 3, 4, 5, 6, 7};
        private readonly Texture2D[] nextTetImage = new Texture2D[5];
        private readonly Rectangle[] nFive = new Rectangle[4];

        private readonly Rectangle[] nFour = new Rectangle[4];

        //TODO: seems like there might be an issue in here, I started a game with a t-block and my next shape was a t-block
        private readonly Rectangle[] nOne = new Rectangle[4];
        private readonly Rectangle[] nThree = new Rectangle[4];
        private readonly Rectangle[] nTwo = new Rectangle[4];
        private readonly List<int> shapeList = new() {1, 2, 3, 4, 5, 6, 7};
        private int nextShape;
        private List<int> nextShapeList = new() {1, 2, 3, 4, 5, 6, 7};
        private int nX = 34;
        private int nY = 50;
        private List<int> shapeQueue = new();
        private int whereAt;

        /// <summary>
        ///     This will set the shape of the next tetris block picture box.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void RenderNextShapes(SpriteBatch spriteBatch)
        {
            //624,53
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise, null,
                Matrix.CreateTranslation(new Vector3(NetworkManager.Instance.Connected ? 670 : 870, 60, 0)));
            for (var i = 0; i < 4; i++)
            {
                spriteBatch.Draw(nextTetImage[0], nOne[i], Color.White);
                spriteBatch.Draw(nextTetImage[1], nTwo[i], Color.White);
                spriteBatch.Draw(nextTetImage[2], nThree[i], Color.White);
                spriteBatch.Draw(nextTetImage[3], nFour[i], Color.White);
                spriteBatch.Draw(nextTetImage[4], nFive[i], Color.White);
            }

            spriteBatch.End();
        }

        public void SetNextShapes()
        {
            FindNextQueue(whereAt);

            SetShape(nextShape, 32, nOne, 0);
            SetShape(shapeQueue[0], 16, nTwo, 1);
            SetShape(shapeQueue[1], 16, nThree, 2);
            SetShape(shapeQueue[2], 16, nFour, 3);
            SetShape(shapeQueue[3], 16, nFive, 4);
        }

        private void SetShape(int shape, int size, Rectangle[] rects, int count)
        {
            var countX = count != 0 ? 7 : 0;
            var countY = count != 0 ? getNextY(count) : 0;

            if (shape == 1)
            {
                nX = 34 + countX;
                nY = 50 + countY;
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX + size, nY, size, size);
                rects[2] = new Rectangle(nX - size, nY, size, size);
                rects[3] = new Rectangle(nX, nY - size, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 2)
            {
                nX = 34 + countX;
                nY = 50 + countY;
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX + size, nY, size, size);
                rects[2] = new Rectangle(nX - size, nY - size, size, size);
                rects[3] = new Rectangle(nX, nY - size, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 3)
            {
                nX = 34 + countX;
                nY = 50 + countY;
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX - size, nY - size, size, size);
                rects[2] = new Rectangle(nX - size, nY, size, size);
                rects[3] = new Rectangle(nX + size, nY, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 4)
            {
                nX = 27 + countX;
                nY = 40 + countY;
                size -= size == 32 ? 8 : 4;
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX + size, nY, size, size);
                rects[2] = new Rectangle(nX - size, nY, size, size);
                rects[3] = new Rectangle(nX + size * 2, nY, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 5)
            {
                nX = 17 + countX + (count != 0 ? 8 : 0);
                nY = 20 + countY + (count != 0 ? 8 : 0);
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX + size, nY + size, size, size);
                rects[2] = new Rectangle(nX, nY - -size, size, size);
                rects[3] = new Rectangle(nX + size, nY, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 6)
            {
                nX = 34 + countX;
                nY = 50 + countY;
                rects[0] = new Rectangle(nX, nY, size, size); // player controlled rect
                rects[1] = new Rectangle(nX + size, nY + -size, size, size);
                rects[2] = new Rectangle(nX - size, nY, size, size);
                rects[3] = new Rectangle(nX + size, nY, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
            else if (shape == 7)
            {
                nX = 34 + countX;
                nY = 50 + countY;
                rects[0] = new Rectangle(nX, nY, size, size);
                rects[1] = new Rectangle(nX + -size, nY, size, size);
                rects[2] = new Rectangle(nX - -size, nY - size, size, size);
                rects[3] = new Rectangle(nX, nY - size, size, size);
                nextTetImage[count] = Globals.BlockTexture[shape - 1];
            }
        }

        public void ResetNext()
        {
            for (var i = 0; i < 4; i++)
            {
                nOne[i] = new Rectangle();
                nTwo[i] = new Rectangle();
                nThree[i] = new Rectangle();
                nFour[i] = new Rectangle();
                nFive[i] = new Rectangle();
            }
        }

        //yikes, kinda a mess. TODO: Maybe find a better way to do this?
        private void FindNextQueue(int count)
        {
            switch (count)
            {
                case 1:
                    shapeQueue = new List<int> {shapeList[1], shapeList[2], shapeList[3], shapeList[4]};
                    break;
                case 2:
                    shapeQueue = new List<int> {shapeList[2], shapeList[3], shapeList[4], shapeList[5]};
                    break;
                case 3:
                    shapeQueue = new List<int> {shapeList[3], shapeList[4], shapeList[5], shapeList[6]};
                    break;
                case 4:
                    shapeQueue = new List<int> {shapeList[4], shapeList[5], shapeList[6], nextShapeList[0]};
                    break;
                case 5:
                    shapeQueue = new List<int> {shapeList[5], shapeList[6], nextShapeList[0], nextShapeList[1]};
                    break;
                case 6:
                    shapeQueue = new List<int> {shapeList[6], nextShapeList[0], nextShapeList[1], nextShapeList[2]};
                    break;
                case 7:
                    shapeQueue = new List<int> {nextShapeList[0], nextShapeList[1], nextShapeList[2], nextShapeList[3]};
                    break;
                case 8:
                    shapeQueue = new List<int> {nextShapeList[1], nextShapeList[2], nextShapeList[3], nextShapeList[4]};
                    break;
                case 9:
                    shapeQueue = new List<int> {nextShapeList[2], nextShapeList[3], nextShapeList[4], nextShapeList[5]};
                    break;
                case 10:
                    shapeQueue = new List<int> {nextShapeList[3], nextShapeList[4], nextShapeList[5], nextShapeList[6]};
                    break;
                case 11:
                    shapeQueue = new List<int> {nextShapeList[4], nextShapeList[5], nextShapeList[6], shapeList[0]};
                    break;
                case 12:
                    shapeQueue = new List<int> {nextShapeList[5], nextShapeList[6], shapeList[0], shapeList[1]};
                    break;
                case 13:
                    shapeQueue = new List<int> {nextShapeList[6], shapeList[0], shapeList[1], shapeList[2]};
                    break;
                case 14:
                    shapeQueue = new List<int> {shapeList[0], shapeList[1], shapeList[2], shapeList[3]};
                    break;
            }
        }

        private int getNextY(int y)
        {
            return y switch
            {
                1 => 75,
                2 => 130,
                3 => 180,
                4 => 230,
                _ => 0
            };
        }

        public void ResetList()
        {
            whereAt = 0;
            nextShapeList = defaultOrder;
            nextShapeList.Shuffle();
            shapeList.Shuffle();
        }

        public int GetNextShape()
        {
            return nextShape;
        }

        public void GenerateNextShape()
        {
            if (whereAt == 7)
            {
                shapeList.Shuffle();
                DebugConsole.Instance.AddMessage($"Next Bag: {shapeList[0]},{shapeList[1]},{shapeList[2]},{shapeList[3]},{shapeList[4]}");
            }

            if (whereAt == 14)
            {
                nextShapeList.Shuffle();
                DebugConsole.Instance.AddMessage($"Next Bag: {nextShapeList[0]},{nextShapeList[1]},{nextShapeList[2]},{nextShapeList[3]},{nextShapeList[4]}");
                whereAt = 0;
            }

            if (whereAt < 7)
                nextShape = shapeList[whereAt];
            else if (whereAt >= 7)
                nextShape = nextShapeList[whereAt - 7];
            whereAt++;
        }

        private static NextShape _instance;
        public static NextShape Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new NextShape();
                }

                return result;
            }
        }
    }
}