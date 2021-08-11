using Microsoft.Xna.Framework;
using Tetris.Main.Player;
using Tetris.Other;

namespace Tetris.Main
{
    public class RotateCheck
    {
        private TetrisPlayer ply;

        public readonly int[] RotPos = { 0, 0, 0, 0, 0, 0 }; // [0] = R1, [1] = R2, [2] = L1, etc.
        /// <summary>
        /// Gives the positions to check for each rotate if the block is unable to stay in its current position
        /// (see https://tetris.fandom.com/wiki/SRS for info) (Note: y on the chart equals -y here)
        /// </summary>
        private readonly int[,,] checkPos =
        {
            {{-1,0},{-1,-1},{0,2},{-1,2}}, // 0 to 1 or 2 to 1
            {{1,0},{1,1},{0,-2},{1,-2}}, // 1 to 0 or 1 to 2
            {{1,0},{1,-1},{0,2},{1,2}}, // 2 to 3 or 0 to 3
            {{-1,0},{-1,1},{0,-2},{-1,-2}} // 3 to 2 or 3 to 0
        };
        private readonly int[,,] checkPosI =
        {
            {{2,0},{-1,0},{2,1},{-1,-2}}, // 0 to 1 or 3 to 2
            {{-2,0},{1,0},{-2,-1},{1,2}}, // 1 to 0 or 2 to 3
            {{1,0},{-2,0},{1,-2},{-2,1}}, // 1 to 2 or 0 to 3
            {{-1,0},{2,0},{-1,2},{2,-1}} // 2 to 1 or 3 to 0
        };

        private readonly Rectangle[] checkBlock = new Rectangle[4];
        private int checkX = 160;
        private int checkY = 16;

        /// <summary>
        /// Checks multiple possible positions for a stuck block to move to(This allows for triple t-spins).
        /// See https://tetris.fandom.com/wiki/SRS for the table that was used.
        /// </summary>
        /// <param name="clockwise"></param>
        private void WallKick(bool clockwise)
        {
            bool failed = false;

            int checkAng = Instance.GetRotate().GetCurAngle() + (clockwise ? -1 : 1); // add or subtract to our rotationAng which gives us our desired rotate
            int checkPosAt = 0;
            string where = "";

            if (checkAng > 3)
                checkAng = 0;
            else if (checkAng < 0)
                checkAng = 3;

            where = $"{checkAng}to{Instance.GetRotate().GetCurAngle()}";

            if (Instance.GetRotate().GetCurShape() == 4) // bit of a hack for I blocks as they don't funciton the same as they do in normal SRS games
            {
                if (checkAng == 2)
                    where = $"0to1";
                if (checkAng == 3)
                    where = "1to0";
            }

            var check = Instance.GetRotate().GetCurShape() == 4 ? checkPosI : checkPos; // because I block is weird, it has its own rotation checks

            UpdateXY();
            UpdateRectangles();

            switch (where)
            {
                case "0to1":
                case "2to1":
                    checkPosAt = 0;
                    break;
                case "1to0":
                case "1to2":
                    checkPosAt = 1;
                    break;
                case "2to3":
                case "0to3":
                    checkPosAt = 2;
                    break;
                case "3to2":
                case "3to0":
                    checkPosAt = 3;
                    break;
            }

            if (IsOutOfBounds()) // if (0,0) is not available
            {
                for (int i = 0; i < 4; i++)
                {
                    checkX += check[checkPosAt, i, 0] * 32;
                    checkY += check[checkPosAt, i, 1] * 32;
                    if (!IsOutOfBounds())
                    {
                        Instance.GetGuiDebug().DebugMessage($"Found position: X:{check[checkPosAt, i, 0] * 32},Y:{check[1, i, 1] * 32}");
                        SetPlayerPosition();
                        break;
                    }
                    UpdateXY();
                    UpdateRectangles();
                    if (i == 3)
                    {
                        failed = true;
                        break;
                    }
                }
            }
            if (!failed)
                SetPlayerPosition();
            else
            {
                Instance.GetRotate().RotatePiece(!clockwise);
                Instance.GetGuiDebug().DebugMessage("Failed to find a position to rotate to.");
            }
        }

        public void UpdateCheck(bool clockwise)
        {
            UpdateXY();

            UpdateRectangles();

            WallKick(clockwise);
        }

        private void UpdateRectangles()
        {
            checkBlock[0] = new Rectangle(checkX, checkY, 32, 32); // player controlled rect
            checkBlock[1] = new Rectangle(checkX + RotPos[0], checkY + RotPos[1], 32, 32);
            checkBlock[2] = new Rectangle(checkX - RotPos[2], checkY - RotPos[3], 32, 32);
            checkBlock[3] = new Rectangle(checkX + RotPos[5], checkY - RotPos[4], 32, 32);
        }

        public bool TSpinLock()
        {
            //A T-Spin Lock is determined if the tetrimino is unable to move left,right,up,or down
            for (int i = 0; i < 4; i++)
            {
                SetAllPositions();
                UpdateRectangles();
                switch (i)
                {
                    case 0:
                        checkX += 32;
                        if (!IsOutOfBounds())
                        {
                            Instance.GetGuiDebug().DebugMessage($"T-Spin failed at check {i} (X+32)");
                            return false;
                        }
                        break;
                    case 1:
                        checkX -= 32;
                        if (!IsOutOfBounds())
                        {
                            Instance.GetGuiDebug().DebugMessage($"T-Spin failed at check {i} (X-32)");
                            return false;
                        }
                        break;
                    case 2:
                        checkY += 32;
                        if (!IsOutOfBounds())
                        {
                            Instance.GetGuiDebug().DebugMessage($"T-Spin failed at check {i} (Y+32)");
                            return false;
                        }
                        break;
                    case 3:
                        checkY -= 32;
                        if (!IsOutOfBounds())
                        {
                            Instance.GetGuiDebug().DebugMessage($"T-Spin failed at check {i} (Y-32)");
                            return false;
                        }
                        break;
                }
            }
            Instance.GetGuiDebug().DebugMessage("T-Spin lock detected!");
            return true;
        }

        private void UpdateXY()
        {
            ply ??= Instance.GetPlayer();
            //set our x,y to the players x,y
            checkX = ply.PlyX;
            checkY = ply.PlyY;
        }

        /// <summary>
        /// Checks to see if the rotated piece is considered out of bounds(inside a block, or off-screen)
        /// </summary>
        /// <returns>true if out of bounds</returns>
        private bool IsOutOfBounds()
        {
            UpdateRectangles();
            Rectangle[] boxes = { checkBlock[0], checkBlock[1], checkBlock[2], checkBlock[3] };

            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].X < 0 || boxes[i].X > 288) // if we are out of screen horizontally
                {
                    return true;
                }
                if (boxes[i].Y > Globals.MaxY) // if we are off screen vertically
                {
                    return true;
                }

                for (int f = ply.PlacedRect.Length - 1; f > 0; f--) // if we are inside a block
                    if (boxes[i].Contains(ply.PlacedRect[f]))
                    {
                        return true;
                    }
            }

            return false;
        }

        public Rectangle[] GetRotationBlocks()
        {
            return new[] { checkBlock[0], checkBlock[1], checkBlock[2], checkBlock[3] };
        }

        public void SetAllPositions()
        {
            if (ply == null)
            {
                ply = Instance.GetPlayer();
                return;
            }
            //set our positions
            for (int i = 0; i < 4; i++)
                checkBlock[i] = ply.Player[i];
            checkX = ply.PlyX;
            checkY = ply.PlyY;
            for (int i = 0; i < 6; i++)
                RotPos[i] = ply.PlayerPos[i];
        }

        private void SetPlayerPosition()
        {
            //set the players position to ours
            for (int i = 0; i < 4; i++)
                ply.Player[i] = checkBlock[i];
            ply.PlyX = checkX;
            ply.PlyY = checkY;
            for (int i = 0; i < 6; i++)
                ply.PlayerPos[i] = RotPos[i];
        }
    }
}