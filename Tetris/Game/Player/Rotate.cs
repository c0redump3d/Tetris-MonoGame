using System.Net.Security;
using Tetris.Multiplayer.Network;

namespace Tetris.Game.Player
{
    public class Rotate
    {
        /// <summary>
        /// This contains all position offsets for each tetris block.
        /// All values are multiplied by the block size(in this case 32)
        /// </summary>
        public readonly int[,,] Blocks =
        {
            {{1, 0, 1, 0, 1, 0}, {1, 0, 0, -1, 1, 0}, {1, 0, 1, 0, -1, 0}, {-1, 0, 0, 1, -1, 0}}, // t block
            {{1, 0, 1, 1, 1, 0}, {1, 0, 0, -1, 1, 1}, {0, 1, 1, 0, -1, 1}, {-1, 1, 1, 0, 1, 0}}, // z block
            {{-1, -1, 1, 0, 0, 1}, {0, 1, 0, 1, 1, 1}, {1, 1, 1, 0, 0, 1}, {-1, 1, 0, 1, -1, 0}}, // j block
            {{1, 0, 1, 0, 0, 2}, {0, 2, 0, -1, 1, 0}, {1, 0, 1, 0, 0, -2}, {0, -2, 0, -1, 1, 0}}, // i block
            {
                {1, 1, 0, -1, 0, 1}, {1, 1, 0, -1, 0, 1}, {1, 1, 0, -1, 0, 1}, {1, 1, 0, -1, 0, 1}
            }, // o block (seems redundant, I know, but this makes my life easier)
            {{1, -1, 1, 0, 0, 1}, {0, 1, 0, 1, -1, 1}, {-1, 1, 1, 0, 0, 1}, {-1, -1, 0, 1, -1, 0}}, // l block
            {{-1, 0, -1, 1, 1, 0}, {1, 0, -1, -1, 1, 0}, {1, 0, 1, -1, -1, 0}, {0, 1, 1, 0, 1, -1}} // s block
        };

        private int currentBlock = 1;
        private int rotationAng; // 0 default
        private readonly RotateCheck rotCheck = RotateCheck.Instance;
        private int timesRotated;

        public void RotatePiece(bool clockwise)
        {
            if (clockwise)
            {
                rotationAng++;
                if (rotationAng == 4)
                    rotationAng = 0;
            }
            else
            {
                rotationAng--;
                if (rotationAng == -1)
                    rotationAng = 3;
            }

            for (var i = 0;
                i < 6;
                i++) // sets R1,R2,etc to their values found within the block array(given its rotationAng)
                rotCheck.RotPos[i] = Blocks[currentBlock - 1, rotationAng, i] * 32;
            if (timesRotated < 10 &&
                PlayerController.Instance.Grounded) // important to make sure the timer is enabled first.
            {
                // after the player has rotated 9 times, stop resetting the confirm timer(This makes it so the player cannot constantly rotate a piece forever)
                PlayerController.Instance.ConfirmTime = 750;
                timesRotated++;
            }
        }

        public int TimesRotated()
        {
            return timesRotated;
        }

        public void ResetRot()
        {
            rotationAng = 0;
        }

        public void SetCurAngle(int ang)
        {
            rotationAng = ang;
        }

        public int GetCurAngle()
        {
            return rotationAng;
        }

        public int GetCurShape()
        {
            return currentBlock;
        }

        public void SetCurShape(int block)
        {
            timesRotated = 0;
            currentBlock = block;
        }

        /// <summary>
        ///     Each shape has its own rotation array, all it does is check current rotation
        ///     and sets rectangles to a new position and adds one to rotationAng.
        ///     Checks to make sure the player is able to actually rotate, given the new positions, is done within RotateCheck.cs
        /// </summary>
        public void RotateTetris(bool clockwise = true)
        {
            rotCheck.SetAllPositions(); // set the position of the rotation check
            RotatePiece(clockwise);
            //Collision check the newly rotated block.
            rotCheck.UpdateCheck(clockwise);
            NetworkManager.Instance.SendPacket(5);
        }

        private static Rotate _instance;
        public static Rotate Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new Rotate();
                }

                return result;
            }
        }
    }
}