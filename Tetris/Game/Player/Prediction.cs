using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.InGame;
using Tetris.Game.Managers;

namespace Tetris.Game.Player
{
    /// <summary>
    /// Creates a clone player that has insanely fast gravity time to show where the block would fall to in its current position.
    /// Fun fact:
    /// This was previously not able to be done in WinForms since the application could not update its position fast enough.
    /// </summary>
    public class Prediction
    {
        #region Variables

        private PlayerController ply;

        private Rectangle tOne;
        private Rectangle tTwo;
        private Rectangle tThree;
        private Rectangle tFour;
        private int predX = 160;
        private int predY = 48;
        private int ghostDirection;
        private float ghostAlpha = 0.8F;

        #endregion

        #region Player

        public void BlockCollision()
        {
            ply ??= PlayerController.Instance;

            predX = ply.PlyX;
            tOne.X = ply.PlyX;
            tTwo.X = ply.PlayerBlocks[1].X;
            tThree.X = ply.PlayerBlocks[2].X;
            tFour.X = ply.PlayerBlocks[3].X;

            if (IsColliding())
                predY = ply.PlyY;

            for (var i = 0; i < 25; i++)
            {
                if (!IsColliding())
                    predY += 32;
                else
                    break;
                UpdateRectangles();
            }
        }

        /// <summary>
        ///     Checks if the player is above any blocks within the PlacedRect array.
        /// </summary>
        /// <returns>True if player is above any blocks</returns>
        private bool IsColliding()
        {
            UpdateRectangles();

            if (TetrisBoard.Instance.WouldCollide(new[] {tOne, tTwo, tThree, tFour}))
                return true;

            if (tOne.Y >= Globals.MaxY || tTwo.Y >= Globals.MaxY || tThree.Y >= Globals.MaxY ||
                tFour.Y >= Globals.MaxY)
                return true;

            return false;
        }

        private void UpdateRectangles()
        {
            tOne = new Rectangle(predX, predY, 32, 32); // player controlled rect
            tTwo = new Rectangle(tTwo.X, predY + ply.PlayerPos[1], 32, 32);
            tThree = new Rectangle(tThree.X, predY - ply.PlayerPos[3], 32, 32);
            tFour = new Rectangle(tFour.X, predY - ply.PlayerPos[4], 32, 32);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ply ??= PlayerController.Instance;
            UpdateRectangles();
            //draw player rectangles
            var currentImage = Globals.BlockTexture[Rotate.Instance.GetCurShape() - 1];

            spriteBatch.Draw(currentImage, tOne, Color.White * ghostAlpha);
            spriteBatch.Draw(currentImage, tTwo, Color.White * ghostAlpha);
            spriteBatch.Draw(currentImage, tThree, Color.White * ghostAlpha);
            spriteBatch.Draw(currentImage, tFour, Color.White * ghostAlpha);

            if (InGameManager.Instance.Paused)
                return;

            //This creates a "fade-in-out" effect
            ghostAlpha += 0.01f * ghostDirection;
            if (ghostAlpha > 0.7)
                ghostDirection = -1;
            if (ghostAlpha < 0.3)
                ghostDirection = 1;
        }

        #endregion

        private static Prediction _instance;
        public static Prediction Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new Prediction();
                }

                return result;
            }
        }
    }
}