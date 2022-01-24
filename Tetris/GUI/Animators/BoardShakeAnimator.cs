using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Animators
{
    public class BoardShakeAnimator
    {
        private double shakeRadius = 15;
        private double shakeStartAngle;
        private double timeLeft;
        private bool placedFinished;
        private Vector2 offset = new Vector2(0, 0);
        public bool Animating;
        public double ShakeStart { get; set; }
        public bool Shaking { get; set; }

        public void FinishAnim() => placedFinished = true;
        
        public void Update(GameTime gameTime)
        {
            if (Shaking)
            {
                offset = new Vector2((float) (Math.Sin(shakeStartAngle) * shakeRadius),
                    (float) (Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += 150 + GameManager.Instance.Random.Next(60);
                PlayerController.Instance.PlyY = 9999;
                if (gameTime.ElapsedGameTime.TotalSeconds - ShakeStart > 2F || shakeRadius <= 0)
                {
                    Animating = true;
                    Shaking = false;
                    shakeRadius = 15;
                    shakeStartAngle = 0;
                    Sfx.Instance.PlaySoundEffect(InGameManager.Instance.Winner ? "gamewin" : "gameover");
                }
            }
            
            
            if (!Animating)
                return;
            timeLeft -= gameTime.ElapsedGameTime.Milliseconds;

            if (timeLeft <= 0)
            {
                if (!placedFinished)
                {
                    PlayerController.Instance.PlyY = 9999;
                    TetrisBoard.Instance.MoveDown();
                    timeLeft = 150;
                }
                else
                {
                    timeLeft = 0;
                    InGameManager.Instance.GameOver = true;
                    RichPresence.Instance.UpdatePresence();
                    InGameManager.Instance.EndGame();
                    Animating = false;
                    placedFinished = false;
                }
            }
        }

        public void StartBatch(SpriteBatch gameBatch)
        {
            if (Shaking)
                gameBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null,
                    Matrix.CreateTranslation(offset.X, offset.Y, 0));
            else
                gameBatch.Begin();
        }

        private static BoardShakeAnimator _instance;
        public static BoardShakeAnimator Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new BoardShakeAnimator();
                }

                return result;
            }
        }
    }
}