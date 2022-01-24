using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Player;
using Tetris.Util;

namespace Tetris.GUI.Screens.ScreenAnimations
{
    public class TetrisRain
    {
        private readonly int[] rainBlockImage = new int[30];
        public float FadeTime;
        private int gravity = 500;
        private int[,] rainBlockPos;
        private Rectangle[,] rainBlocks;

        private Random rand;
        private int waitTime = 100;

        public void SetUp()
        {
            rand = new Random();
            rainBlockPos = new int[30, 10];
            rainBlocks = new Rectangle[30, 4];
            for (var i = 0; i < rainBlocks.GetLength(0); i++)
            for (var f = 0; f < rainBlocks.GetLength(1); f++)
                rainBlocks[i, f] = new Rectangle(0, 800, 32, 32);
        }

        /// <summary>
        ///     Chooses a random shape and rotation angle for the given block and resets its position.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="arr"></param>
        private void ChooseBlock(Rectangle[] rect, int arr)
        {
            var shape = rand.Next(0, 7);
            var rotation = rand.Next(0, 3);
            for (var i = 2;
                i < 8;
                i++) // sets R1,R2,etc to their values found within the block array(given its rotationAng)
                rainBlockPos[arr, i] = Rotate.Instance.Blocks[shape, rotation, i - 2] * 32;
            rect[0] = new Rectangle(rainBlockPos[arr, 0], rainBlockPos[arr, 1], 32, 32);
            rect[1] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr, 2],
                rainBlockPos[arr, 1] + rainBlockPos[arr, 3], 32, 32);
            rect[2] = new Rectangle(rainBlockPos[arr, 0] - rainBlockPos[arr, 4],
                rainBlockPos[arr, 1] - rainBlockPos[arr, 5], 32, 32);
            rect[3] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr, 7],
                rainBlockPos[arr, 1] - rainBlockPos[arr, 6], 32, 32);

            var randX = RandX();
            rainBlockPos[arr, 0] = randX; // set to rand x
            for (var l = 0; l < 40; l++) // checks each x position
            {
                UpdateRectangles(rect, arr);
                //move any blocks that are off-screen
                for (var i = 0; i < rect.Length; i++)
                {
                    if (rect[i].X < 0) rainBlockPos[arr, 0] += 32;

                    if (rect[i].X > 1248) rainBlockPos[arr, 0] -= 32;
                }
            }

            rainBlockPos[arr, 1] = FindY(rainBlockPos[arr, 0], Globals.TopOut - 80, arr); // set to rand x
            UpdateRectangles(rect, arr);

            rainBlockImage[arr] = shape;

            for (var i = 0; i < rect.Length; i++)
                //set the rain block to the new block
                rainBlocks[arr, i] = rect[i];
        }

        private bool DoesBlockExistHere(int x, int y, int arr)
        {
            //pretty self-explanatory, checks to see if any rain blocks collide with the checked block
            var rect = new Rectangle[4];
            for (var g = 0; g < rect.Length; g++)
            {
                rect[0] = new Rectangle(x, y, 32, 32);
                rect[1] = new Rectangle(x + rainBlockPos[arr, 2], y + rainBlockPos[arr, 3], 32, 32);
                rect[2] = new Rectangle(x - rainBlockPos[arr, 4], y - rainBlockPos[arr, 5], 32, 32);
                rect[3] = new Rectangle(x + rainBlockPos[arr, 7], y - rainBlockPos[arr, 6], 32, 32);
                for (var i = 0; i < rainBlocks.GetLength(0); i++)
                for (var f = 0; f < rainBlocks.GetLength(1); f++)
                    if (rainBlocks[i, f].Intersects(rect[g]))
                        return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks for a y position for the block to spawn at that will not interfere with other blocks.
        /// </summary>
        /// <param name="x">Blocks x-pos</param>
        /// <param name="y">Blocks y-pos</param>
        /// <param name="arr">Blocks array index</param>
        /// <returns>Safe y-position to spawn at</returns>
        private int FindY(int x, int y, int arr)
        {
            for (var i = 0; i < 23; i++)
                if (!DoesBlockExistHere(x, y - i * 32, arr))
                    return y - i * 32;

            return y;
        }

        private void UpdateRectangles(Rectangle[] rect, int arr)
        {
            rect[0] = new Rectangle(rainBlockPos[arr, 0], rainBlockPos[arr, 1], 32, 32);
            rect[1] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr, 2],
                rainBlockPos[arr, 1] + rainBlockPos[arr, 3], 32, 32);
            rect[2] = new Rectangle(rainBlockPos[arr, 0] - rainBlockPos[arr, 4],
                rainBlockPos[arr, 1] - rainBlockPos[arr, 5], 32, 32);
            rect[3] = new Rectangle(rainBlockPos[arr, 0] + rainBlockPos[arr, 7],
                rainBlockPos[arr, 1] - rainBlockPos[arr, 6], 32, 32);
        }

        private int RandX()
        {
            //makes the x position a bit more randomized
            var positions = new List<int>();
            for (var i = 0; i < 40; i++)
                positions.Add(i * 32);

            positions.Shuffle();
            var randX = rand.Next(0, positions.Count);

            return positions[randX];
        }

        /// <summary>
        ///     Finds any blocks that are no longer shown on screen to reuse for the rain.
        /// </summary>
        private void SearchForBlock()
        {
            for (var i = 0; i < rainBlocks.GetLength(0); i++)
            {
                var found = 0;
                for (var f = 0; f < rainBlocks.GetLength(1); f++)
                {
                    if (rainBlocks[i, f].Y > 736) // no longer on screen
                    {
                        found++;
                        if (found != 4)
                            continue;
                    }

                    //if the whole tetris block is off screen
                    if (found == 4)
                    {
                        Rectangle[] pieces =
                            {rainBlocks[i, 0], rainBlocks[i, 1], rainBlocks[i, 2], rainBlocks[i, 3]};
                        ChooseBlock(pieces, i);
                        waitTime = rand.Next(100, 1000);
                        break;
                    }
                }

                if (found == 4)
                    break;
            }
        }

        private void Gravity(GameTime gameTime)
        {
            gravity -= gameTime.ElapsedGameTime.Milliseconds;

            if (gravity <= 0)
            {
                for (var i = 0; i < rainBlocks.GetLength(0); i++)
                for (var f = 0; f < rainBlocks.GetLength(1); f++)
                    rainBlocks[i, f].Y += 32;

                gravity = 500;
            }
        }

        public void UpdateRain(GameTime gameTime)
        {
            //Fade timer when menu is shown
            if (FadeTime < 1f) FadeTime += 0.04f;

            Gravity(gameTime);

            waitTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (waitTime <= 0) SearchForBlock();
        }

        public void DrawRain(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw the rain!
            for (var i = 0; i < rainBlocks.GetLength(0); i++)
            for (var f = 0; f < rainBlocks.GetLength(1); f++)
                spriteBatch.Draw(Globals.BlockTexture[rainBlockImage[i]], rainBlocks[i, f],
                    Color.Lerp(Color.White, Color.Black, 0.3f) * FadeTime);
        }

        private static TetrisRain _instance;
        public static TetrisRain Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new TetrisRain();
                }

                return result;
            }
        }
    }
}