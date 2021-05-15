using System;
using Microsoft.Xna.Framework;
using Tetris.GameDebug;
using Tetris.Other;

namespace Tetris.Main
{
    public class GameGlobal
    {
        public bool Paused { get; set; }
        public bool Stopped { get; set; }
        public bool GameOver { get; set; }
        /// <summary>
        /// Returns the current screen that is being displayed
        /// 0 - Main Menu, 1 - In-Game, 2 - Game Over
        /// </summary>
        public int CurrentScreen { get; set; }
        public bool ScreenShake { get; set; }
        public double ShakeStart { get; set; }
        public bool Winner { get; set; }
        public bool Sender { get; set; }
        private bool isCountdown = false;

        public GameGlobal()
        {
            Stopped = true;
        }

        private void ResetGame()
        {
            Instance.GetPlayer().SetGravity(Instance.GetScoreHandler().Level);
            //wipe all placed rectangles
            Instance.GetPlayer().Reset();

            if (Instance.GetScoreHandler().Level > 6)
                Instance.GetPlayer().RandomBlock(Instance.GetScoreHandler().Level - 3);

            //GameBoard.Invalidate();
        }
        
        public void StartGame()
        {
            //hides labels, starts timers and starts to play music
            GameOver = false;
            Winner = false;
            Sender = false;
            ResetGame();
            Paused = false;
            Stopped = false;
            Instance.GetGui().ClearButtons();
            Instance.GetSound().PlayMusic(Instance.GetScoreHandler().Level);
        }
        
        public void StartCountdown()
        {
            CurrentScreen = 1;
            Instance.GetScoreHandler().Reset();
            //reset stats
            string addZero = Instance.GetScoreHandler().Level < 10 ? "0" : "";
            Instance.GetGui().LevelText = $@"{addZero}{Instance.GetScoreHandler().Level}";
            Instance.GetGui().ScoreText = @"0";
            Instance.GetGui().LineText = @"000";
            Instance.GetGui().ClearButtons(); // clear all buttons on screen
            Animate.StartCountdown(0); // start our countdown
            isCountdown = true;
        }
        
        public bool GetCountdown()
        {
            return isCountdown;
        }
        
        public void EndCountdown()
        {
            isCountdown = false;
        }
        
        public void EndGame()
        {
            if (!GameOver)
                GameOver = true;
            Instance.GetPlayer().PlacedRect = new Rectangle[0];
            //GravityTimer.Stop(); // stop gravity
            Paused = true; // pause game
            Stopped = true; // stop music
            Instance.GetSound().StopMusic();
            Instance.GetGui().AddGameOverButton();
            //if(!InstanceManager.InMultiplayer)
            //Instance.GetSound().PlaySoundEffect("gameover");
            Debug.DebugMessage("GAME: End", 1);
        }
    }
}