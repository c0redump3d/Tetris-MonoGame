using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Other;

namespace Tetris.Main.Player
{
    public class Prediction
    {
        #region Variables

        private TetrisPlayer ply;

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
            ply ??= Instance.GetPlayer();

            predX = ply.PlyX;
            tOne.X = ply.PlyX;
            tTwo.X = ply.Player[1].X;
            tThree.X = ply.Player[2].X;
            tFour.X = ply.Player[3].X;
            
            if(IsColliding())
                predY = ply.PlyY;
            
            for (int i = 0; i < 25; i++)
            {
                if (!IsColliding())
                    predY += 32;
                else
                    break;
                UpdateRectangles();
            }
        }

        /// <summary>
        /// Checks if the player is above any blocks within the PlacedRect array.
        /// </summary>
        /// <returns>True if player is above any blocks</returns>
        private bool IsColliding()
        {
            UpdateRectangles();
            
            for (int i = ply.PlacedRect.Length - 1; i > 0; i--)
                if (tOne.Y == ply.PlacedRect[i].Y - 32 && tOne.X == ply.PlacedRect[i].X
                    || tTwo.Y == ply.PlacedRect[i].Y - 32 && tTwo.X == ply.PlacedRect[i].X
                    || tThree.Y == ply.PlacedRect[i].Y - 32 && tThree.X == ply.PlacedRect[i].X
                    || tFour.Y == ply.PlacedRect[i].Y - 32 && tFour.X == ply.PlacedRect[i].X)
                    return true;

            if (tOne.Y >= Globals.MaxY || tTwo.Y >= Globals.MaxY || tThree.Y >= Globals.MaxY ||
                tFour.Y >= Globals.MaxY)
                return true;

            return false;
        }

        private void UpdateRectangles()
        {
            tOne = new(predX, predY, 32, 32); // player controlled rect
            tTwo = new(tTwo.X, predY + ply.PlayerPos[1], 32, 32);
            tThree = new(tThree.X, predY - ply.PlayerPos[3], 32, 32);
            tFour = new(tFour.X, predY - ply.PlayerPos[4], 32, 32);
        }
        
        public void Draw(SpriteBatch _spriteBatch)
        {
            ply ??= Instance.GetPlayer();
            UpdateRectangles();
            //draw player rectangles
            Texture2D currentImage = Globals.BlockPlacedTexture[Instance.GetRotate().GetCurShape()-1];
            
            _spriteBatch.Draw(currentImage, tOne, Color.White * ghostAlpha);
            _spriteBatch.Draw(currentImage, tTwo, Color.White * ghostAlpha);
            _spriteBatch.Draw(currentImage, tThree, Color.White * ghostAlpha);
            _spriteBatch.Draw(currentImage, tFour, Color.White * ghostAlpha);

            if (Instance.GetGame().Paused)
                return;
            
            //This creates a "fade-in-out" effect
            ghostAlpha += 0.01f * ghostDirection;
            if (ghostAlpha > 0.7)
                ghostDirection = -1;
            if (ghostAlpha < 0.3)
                ghostDirection = 1;
        }

        #endregion
    }
}