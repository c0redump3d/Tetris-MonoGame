using Microsoft.Xna.Framework;
using Tetris.Game.InGame;
using Tetris.Multiplayer.Network;
using Tetris.Sound;

namespace Tetris.Game.Player
{
    public class Movement
    {
        private Rectangle bFour;
        private Rectangle bOne;
        private Rectangle bThree;
        private Rectangle bTwo;

        private int plyX;
        private int plyY;

        private void UpdateLoc()
        {
            PlayerController.Instance.UpdateRectangles();
            bOne = PlayerController.Instance.PlayerBlocks[0];
            bTwo = PlayerController.Instance.PlayerBlocks[1];
            bThree = PlayerController.Instance.PlayerBlocks[2];
            bFour = PlayerController.Instance.PlayerBlocks[3];
            plyX = PlayerController.Instance.PlyX;
            plyY = PlayerController.Instance.PlyY;
        }

        /// <summary>
        ///     Moves tetris block right one tile.
        /// </summary>
        public void MoveRight()
        {
            UpdateLoc();
            if (TetrisBoard.Instance.WouldCollideLR(new[] {bOne, bTwo, bThree, bFour}, false))
                return;
            plyX += 32;
            Sfx.Instance.PlaySoundEffect("move");
            PlayerController.Instance.PlyX = plyX;
            PlayerController.Instance.PlyY = plyY;
            NetworkManager.Instance.SendPacket(3);
        }

        /// <summary>
        ///     Moves tetris block left one tile.
        /// </summary>
        public void MoveLeft()
        {
            UpdateLoc();
            if (TetrisBoard.Instance.WouldCollideLR(new[] {bOne, bTwo, bThree, bFour}, true))
                return;

            plyX -= 32;
            Sfx.Instance.PlaySoundEffect("move");
            PlayerController.Instance.PlyX = plyX;
            PlayerController.Instance.PlyY = plyY;
            NetworkManager.Instance.SendPacket(3);
        }

        /// <summary>
        ///     Moves tetris block down a tile.
        /// </summary>
        /// <returns></returns>
        public void MoveDown()
        {
            UpdateLoc();
            if (PlayerController.Instance.IsColliding())
                return;
            plyY += 32;
            ScoreHandler.Instance.Score++;
            Sfx.Instance.PlaySoundEffect("move");
            PlayerController.Instance.PlyX = plyX;
            PlayerController.Instance.PlyY = plyY;
            NetworkManager.Instance.SendPacket(3);
        }

        private static Movement _instance;
        public static Movement Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new Movement();
                }

                return result;
            }
        }
    }
}