using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.InGame;
using Tetris.Game.Mode;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.GUI.Animators;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Particle;
using Tetris.GUI.Screens;
using Tetris.Multiplayer;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.Managers
{
    public class InGameManager
    {
        
        public readonly Viewport NormalViewport = new (480, 32, 320, 656);
        public readonly Viewport MultiViewport = new (279, 32, 320, 656);
        private bool showPinch;
        private int pinchDirection;
        private float pinchOpacity;

        public bool Paused { get; private set; }
        public bool Stopped { get; private set; } = true;
        public bool GameOver { get; set; }
        public bool CanMove => !BoardShakeAnimator.Instance.Shaking && !BoardShakeAnimator.Instance.Animating;
        public bool Waiting => PlayerController.Instance.Frozen;
        public bool Winner { get; set; }
        private bool Started { get; set; }
        public bool IsCountdown { get; set; }

        public void StartGame()
        {
            if (Started)
            {
                Sfx.Instance.PlayPauseMusic(ScoreHandler.Instance.Level);
                IsCountdown = false;
                Paused = false;
                return;
            }

            //hides labels, starts timers and starts to play music
            GameOver = false;
            Winner = false;
            PlayerController.Instance.Reset();
            ModeManager.Instance.GetCurrentMode().OnGameStart(); // game start event for modes
            Paused = false;
            Stopped = false;
            Sfx.Instance.PlayMusic(ScoreHandler.Instance.Level);
            PlayerController.Instance.SetGravity(ScoreHandler.Instance.Level);

            RichPresence.Instance.UpdatePresence(); // update discord rpc to reflect new level/score.
            Started = true;
        }

        public void PauseGame()
        {
            if (!Paused)
            {
                Paused = !Paused;
                Sfx.Instance.PlayPauseMusic(ScoreHandler.Instance.Level);
                Sfx.Instance.PlaySoundEffect("pause");
            }
            else
            {
                if (!GetCountdown())
                    StartCountdown();
            }

            RichPresence.Instance.UpdatePresence();
        }

        public void StartCountdown()
        {
            if (Started)
            {
                Animate.StartCountdown(0); // start our countdown
                IsCountdown = true;
                return;
            }

            RichPresence.Instance.SetPresence(1);
            ScoreHandler.Instance.Reset();
            //reset stats
            var addZero = ScoreHandler.Instance.Level < 10 ? "0" : "";
            ScoreHandler.Instance.ScoreFont = Globals.Hoog28;
            ScoreHandler.Instance.LevelText = $@"{addZero}{ScoreHandler.Instance.Level}";
            ScoreHandler.Instance.ScoreText = @"0";
            ScoreHandler.Instance.LineText = @"000";
            Animate.StartCountdown(0); // start our countdown
            IsCountdown = true;
        }

        public void DrawGame(SpriteBatch gameBatch, SpriteBatch screenBatch)
        {
            if (Gui.Instance.CurrentScreen is GuiInGame && !Stopped)
            {
                if(NetworkManager.Instance.Connected)
                    PlayerMP.Instance.Draw(gameBatch);
                gameBatch.GraphicsDevice.Viewport = !NetworkManager.Instance.Connected ? NormalViewport : MultiViewport;
                BoardShakeAnimator.Instance.StartBatch(gameBatch);
                Prediction.Instance.Draw(gameBatch);
                PlayerController.Instance.DrawPlayer(gameBatch);
                DrawPinch(gameBatch);
                gameBatch.End();
                gameBatch.GraphicsDevice.Viewport = TetrisGame.Instance.DefaultViewport;
                ParticleManager.Instance.DrawParticles(gameBatch);
                NextShape.Instance.RenderNextShapes(screenBatch);
                HoldShape.Instance.DrawHoldShape(screenBatch);
            }

            gameBatch.GraphicsDevice.Viewport = !NetworkManager.Instance.Connected ? new Viewport(480, 32, 320, 656) : MultiViewport;
            gameBatch.Begin();
            Animate.UpdateAnimation(gameBatch);
            gameBatch.End();
            MessageAnimator.Instance.Draw(gameBatch);
            gameBatch.GraphicsDevice.Viewport = TetrisGame.Instance.DefaultViewport;
        }

        public void UpdateGame(GameTime gameTime)
        {
            if (InGameManager.Instance.Stopped && InGameManager.Instance.Paused)
                return;
            MessageAnimator.Instance.Update(gameTime);
            PlayerController.Instance.Gravity(gameTime);
            PlayerController.Instance.Update(gameTime);
            BoardShakeAnimator.Instance.Update(gameTime);
            Prediction.Instance.BlockCollision();
            ScoreHandler.Instance.Update();
            UpdatePinch();
        }

        private void DrawPinch(SpriteBatch spriteBatch)
        {
            if (showPinch)
            {
                spriteBatch.Draw(Globals.PinchOverlay, new Vector2(0, 0), Color.White * pinchOpacity);

                pinchOpacity += 0.02f * pinchDirection;
                if (pinchOpacity > 1.0)
                    pinchDirection = -1;
                if (pinchOpacity < 0.3)
                    pinchDirection = 1;
            }
        }

        private void UpdatePinch()
        {
            var found = false;
            for (var i = 0; i < TetrisBoard.Instance.Board.GetLength(0); i++)
                if (i * 32 + Globals.LowestY < 128)
                    for (var f = 0; f < 10; f++)
                        if (TetrisBoard.Instance.Board[i, f] != 0)
                            found = true;

            if ((found || ModeManager.Instance.GetCurrentMode().ShowPinch) &&
                CanMove && !Paused)
            {
                showPinch = true;
                Sfx.Instance.PlayPinch();
            }
            else
            {
                showPinch = false;
                Sfx.Instance.StopPinch();
            }

        }
        
        public bool GetCountdown()
        {
            return IsCountdown;
        }

        public void EndCountdown()
        {
            IsCountdown = false;
        }

        public void EndGame()
        {
            if (!GameOver)
                GameOver = true;
            showPinch = false;
            Paused = true; // pause game
            Stopped = true; // stop music
            Sfx.Instance.StopMusic();
            Gui.Instance.SetCurrentScreen(new GuiGameOver());
            Started = false;
            DebugConsole.Instance.AddMessage("Ended Game");
        }

        private static InGameManager _instance;
        public static InGameManager Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new InGameManager();
                }

                return result;
            }
        }
    }
}