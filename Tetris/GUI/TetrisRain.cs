using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Other;

namespace Tetris.GUI
{
    public class TetrisRain
    {
        private int[,] rainBlockPos =
        {
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0}
        };

        private Rectangle[,] rainBlocks =
        {
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()},
            {new(),new(),new(),new()}
        };

        private int[] rainBlockImage = new int[15];
        public float FadeTime = 0f;
        private int waitTime = 1500;
        private int gravity = 500;
        
        private Random rand;
        
        public void SetUp()
        {
            rand = new Random();
            for (int i = 0; i < rainBlocks.GetLength(0); i++)
            {
                for (int f = 0; f < rainBlocks.GetLength(1); f++)
                {
                    rainBlocks[i, f] = new Rectangle(0,Globals.MaxY+32,32,32);
                }
            }
        }

        private void ChooseBlock(Rectangle[] rect, int arr)
        {
            int shape = rand.Next(0, 7);
            int rotation = rand.Next(0, 3);
            for (int i = 2; i < 8; i++) // sets R1,R2,etc to their values found within the block array(given its rotationAng)
                rainBlockPos[arr,i] = Instance.GetRotate().Blocks[shape, rotation, i-2] * 32;
            rect[0] = new Rectangle(rainBlockPos[arr, 0], rainBlockPos[arr, 1], 32, 32);
            rect[1] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr,2], rainBlockPos[arr, 1] + rainBlockPos[arr,3], 32, 32);
            rect[2] = new Rectangle(rainBlockPos[arr, 0] - rainBlockPos[arr,4], rainBlockPos[arr, 1] - rainBlockPos[arr,5], 32, 32);
            rect[3] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr,7], rainBlockPos[arr, 1] - rainBlockPos[arr,6], 32, 32);

            int randX = RandX();
            rainBlockPos[arr, 0] = randX; // set to rand x


            for (int l = 0; l < 10; l++)
            {
                UpdateRectangles(rect, arr);
                for (int i = 0; i < rect.Length; i++)
                {
                    if (rect[i].X < 0)
                    {
                        rainBlockPos[arr, 0] += 32;
                    }

                    if (rect[i].X > 288)
                    {
                        rainBlockPos[arr, 0] -= 32;
                    }
                }
            }
            rainBlockPos[arr, 1] = FindY(rainBlockPos[arr,0], Globals.TopOut - 64, arr); // set to rand x
            UpdateRectangles(rect, arr);
            
            rainBlockImage[arr] = shape;

            for (int i = 0; i < rect.Length; i++)
            {
                rainBlocks[arr, i] = rect[i];
            }
        }

        private bool DoesBlockExistHere(int x, int y, int arr)
        {
            Rectangle[] rect = new Rectangle[4];
            for (int g = 0; g < rect.Length; g++)
            {
                rect[0] = new Rectangle(x, y, 32, 32);
                rect[1] = new Rectangle(x + rainBlockPos[arr,2], y + rainBlockPos[arr,3], 32, 32);
                rect[2] = new Rectangle(x - rainBlockPos[arr,4], y - rainBlockPos[arr,5], 32, 32);
                rect[3] = new Rectangle(x + rainBlockPos[arr,7], y - rainBlockPos[arr,6], 32, 32);
                for (int i = 0; i < rainBlocks.GetLength(0); i++)
                {
                    for (int f = 0; f < rainBlocks.GetLength(1); f++)
                    {
                        if (rainBlocks[i, f].Intersects(rect[g]))
                            return true;
                    }
                }
            }

            return false;
        }

        private int FindY(int x,int y,int arr)
        {
            for (int i = 0; i < 20; i++)
            {
                if (!DoesBlockExistHere(x, y - (i * 32), arr))
                    return y - (i * 32);
            }

            return y;
        }
        
        private void UpdateRectangles(Rectangle[] rect, int arr)
        {
            rect[0] = new Rectangle(rainBlockPos[arr, 0], rainBlockPos[arr, 1], 32, 32);
            rect[1] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr,2], rainBlockPos[arr, 1] + rainBlockPos[arr,3], 32, 32);
            rect[2] = new Rectangle(rainBlockPos[arr, 0] - rainBlockPos[arr,4], rainBlockPos[arr, 1] - rainBlockPos[arr,5], 32, 32);
            rect[3] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr,7], rainBlockPos[arr, 1] - rainBlockPos[arr,6], 32, 32);
        }
        
        private int RandX()
        {
            List<int> positions = new List<int>();
            for(int i = 0; i < 10; i++)
                positions.Add(i*32);

            positions.Shuffle();
            int randX = rand.Next(0, positions.Count);

            return positions[randX];
        }

        private void SearchForBlock()
        {
            for (int i = 0; i < rainBlocks.GetLength(0); i++)
            {
                int found = 0;
                for (int f = 0; f < rainBlocks.GetLength(1); f++)
                {
                    if (rainBlocks[i, f].Y > Globals.MaxY + 32)
                    {
                        found++;
                        if (found != 4)
                            continue;
                    }

                    if (found == 4)
                    {
                        Rectangle[] pieces =
                            {rainBlocks[i, 0], rainBlocks[i, 1], rainBlocks[i, 2], rainBlocks[i, 3]};
                        ChooseBlock(pieces, i);
                        waitTime = rand.Next(100, 2000);
                        break;
                    }
                }

                if (found == 4)
                    break;
            }
        }

        private void Gravity()
        {
            for (int i = 0; i < rainBlocks.GetLength(0); i++)
            {
                for (int f = 0; f < rainBlocks.GetLength(1); f++)
                {
                    rainBlocks[i, f].Y += 1;
                }
            }
        }

        public void DrawRain(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
            if (FadeTime < 0.8f)
            {
                FadeTime += 0.04f;
            }
            
            waitTime -= gameTime.ElapsedGameTime.Milliseconds;
            
            for (int i = 0; i < rainBlocks.GetLength(0); i++)
            {
                for (int f = 0; f < rainBlocks.GetLength(1); f++)
                {
                    spriteBatch.Draw(Globals.BlockTexture[rainBlockImage[i]], rainBlocks[i, f], Color.White * FadeTime);
                }
            }

            if (waitTime <= 0)
            {
                SearchForBlock();
            }
            
            Gravity();
            
        }
    }
}