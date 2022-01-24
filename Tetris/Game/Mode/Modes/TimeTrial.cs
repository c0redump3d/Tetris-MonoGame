using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;

namespace Tetris.Game.Mode.Modes
{
    public class TimeTrial : CustomMode
    {
        private int minutesRemaining = 2;
        private int secondsRemaining = 59;
        private int timerWait = 1000;

        public TimeTrial(string name, string objective) : base(name, objective)
        {
            LevelUp = false;
        }

        public override void OnGameStart()
        {
            minutesRemaining = 2;
            secondsRemaining = 59;
            base.OnGameStart();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var secondsText = secondsRemaining < 10 ? $"0{secondsRemaining}" : $"{secondsRemaining}";

            spriteBatch.DrawString(Globals.Hoog12,
                $"Time Remaining: {minutesRemaining}:{secondsText}",
                new Vector2(0, 0), Color.White);
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            timerWait -= gameTime.ElapsedGameTime.Milliseconds;
            if (timerWait <= 0 && InGameManager.Instance.CanMove)
            {
                timerWait = 1000;
                if (secondsRemaining != 0)
                {
                    secondsRemaining--;
                }
                else
                {
                    if (minutesRemaining != 0)
                    {
                        secondsRemaining = 59;
                        minutesRemaining--;
                    }
                }
            }

            if (minutesRemaining == 0 && secondsRemaining <= 30)
            {
                ShowPinch = true;
            }

            if (minutesRemaining == 0 && secondsRemaining == 0 && InGameManager.Instance.CanMove)
            {
                InGameManager.Instance.Winner = true;
                TetrisBoard.Instance.AddBlockToBoard(8, -32, Globals.TopOut);
                PlayerController.Instance.EndGame(gameTime);
            }

            base.Update(gameTime);
        }
    }
}