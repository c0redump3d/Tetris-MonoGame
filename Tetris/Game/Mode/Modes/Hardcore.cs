using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.InGame;
using Tetris.Game.Managers;

namespace Tetris.Game.Mode.Modes
{
    public class Hardcore : CustomMode
    {
        public Hardcore(string name, string objective) : base(name, objective){}

        public override void OnGameStart()
        {
            //If the level selected is higher than ten, limit it to 8 rows generated to make it more fair.
            var rowsToGenerate = ScoreHandler.Instance.Level > 10 ? 8 : ScoreHandler.Instance.Level - 3;

            if (ScoreHandler.Instance.Level > 3)
                TetrisBoard.Instance.RandomBlock(rowsToGenerate);
            base.OnGameStart();
        }

        public override void OnRowRemove()
        {
            //for hardcore mode
            if (TetrisBoard.Instance.GetActualLines() is > 1 and < 4 &&
                !ScoreHandler.Instance.WasTSpin)
                if (TetrisBoard.Instance.GetTotalLines() != 4) // don't punish for getting a tetris
                    TetrisBoard.Instance.RandomBlock(TetrisBoard.Instance.GetActualLines() - 1);
            base.OnRowRemove();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}