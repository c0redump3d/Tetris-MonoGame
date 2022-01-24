using Microsoft.Xna.Framework;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;

namespace Tetris.Game.Mode.Modes
{
    public class LineGoal : CustomMode
    {
        public LineGoal(string name, string objective) : base(name, objective)
        {
            LevelUp = false;
        }

        public override void Update(GameTime gameTime)
        {
            if(ScoreHandler.Instance.TotalLines >= 40 &&
               InGameManager.Instance.CanMove)
            {
                InGameManager.Instance.Winner = true;
                TetrisBoard.Instance.AddBlockToBoard(8, -32, Globals.TopOut);
                PlayerController.Instance.EndGame(gameTime);
            }
            base.Update(gameTime);
        }
    }
}