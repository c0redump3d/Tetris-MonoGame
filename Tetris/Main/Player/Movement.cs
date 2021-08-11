using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Other;

namespace Tetris.Main.Player
{
    public class Movement
    {
        private Rectangle bOne;
        private Rectangle bTwo;
        private Rectangle bThree;
        private Rectangle bFour;
        private Rectangle[] placedRect;

        private int plyX;
        private int plyY;

        private void UpdateLoc()
        {
            bOne = Instance.GetPlayer().Player[0];
            bTwo = Instance.GetPlayer().Player[1];
            bThree = Instance.GetPlayer().Player[2];
            bFour = Instance.GetPlayer().Player[3];
            placedRect = Instance.GetPlayer().PlacedRect;
            plyX = Instance.GetPlayer().PlyX;
            plyY = Instance.GetPlayer().PlyY;
        }

        /// <summary>
        /// Moves tetris block right one tile.
        /// </summary>
        public void MoveRight()
        {
            UpdateLoc();
            for (int i = placedRect.Length - 1; i > 0; i--)
                if (bOne.X == placedRect[i].X - 32 && bOne.Y == placedRect[i].Y
                    || bTwo.X == placedRect[i].X - 32 && bTwo.Y == placedRect[i].Y
                    || bThree.X == placedRect[i].X - 32 && bThree.Y == placedRect[i].Y
                    || bFour.X == placedRect[i].X - 32 && bFour.Y == placedRect[i].Y
                    || bOne.X >= 288
                    || bTwo.X >= 288
                    || bThree.X >= 288
                    || bFour.X >= 288)
                {
                    return;
                }
            plyX += 32;
            Instance.GetSound().PlaySoundEffect("move");
            Instance.GetPlayer().PlyX = plyX;
            Instance.GetPlayer().PlyY = plyY;
            Instance.GetPacket().SendPacketFromName("pos");
        }

        /// <summary>
        /// Moves tetris block left one tile.
        /// </summary>
        public void MoveLeft()
        {
            UpdateLoc();
            for (int i = placedRect.Length - 1; i > 0; i--)
                if (bOne.X == placedRect[i].X + 32 && bOne.Y == placedRect[i].Y
                    || bTwo.X == placedRect[i].X + 32 && bTwo.Y == placedRect[i].Y
                    || bThree.X == placedRect[i].X + 32 && bThree.Y == placedRect[i].Y
                    || bFour.X == placedRect[i].X + 32 && bFour.Y == placedRect[i].Y
                    || bOne.X <= 0
                    || bTwo.X <= 0
                    || bThree.X <= 0
                    || bFour.X <= 0)
                {
                    return;
                }

            plyX -= 32;
            Instance.GetSound().PlaySoundEffect("move");
            Instance.GetPlayer().PlyX = plyX;
            Instance.GetPlayer().PlyY = plyY;
            Instance.GetPacket().SendPacketFromName("pos");

        }

        /// <summary>
        /// Moves tetris block down a tile.
        /// </summary>
        /// <returns></returns>
        public void MoveDown()
        {
            UpdateLoc();
            for (int i = placedRect.Length - 1; i > 0; i--)
                if (bOne.Y == placedRect[i].Y - 32 && bOne.X == placedRect[i].X
                    || bTwo.Y == placedRect[i].Y - 32 && bTwo.X == placedRect[i].X
                    || bThree.Y == placedRect[i].Y - 32 && bThree.X == placedRect[i].X
                    || bFour.Y == placedRect[i].Y - 32 && bFour.X == placedRect[i].X)
                    return;
            
            if (bOne.Y != Globals.MaxY && bTwo.Y != Globals.MaxY
                && bThree.Y != Globals.MaxY && bFour.Y != Globals.MaxY)
            {
                plyY += 32;
                Instance.GetScoreHandler().Score++;
                Instance.GetSound().PlaySoundEffect("move");
                Instance.GetPlayer().PlyX = plyX;
                Instance.GetPlayer().PlyY = plyY;
                Instance.GetPacket().SendPacketFromName("pos");
            }
        }
    }
}
