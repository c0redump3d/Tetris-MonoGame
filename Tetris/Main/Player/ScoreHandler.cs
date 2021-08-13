using System;
using Microsoft.Xna.Framework;
using Tetris.Other;

namespace Tetris.Main.Player
{
    public class ScoreHandler
    {
        public int TotalLines = 0;
        public int Score = 0;
        public int[] Bonuses = new int[4];
        public int Level = 1;
        public int SelectedLevel = 1;
        private int points = 40;
        private int lastLevel = 1;
        private int bonus = 0;
        public bool Remove = false;
        private bool leveledUp = false;
        public bool WasTSpin { get; set; }

        public void Update()
        {
            if (Remove)
                CleanUp();
            
            var scoreText = Score > 999999 ? "999,999" : $"{Score:n0}";
            var font = Score > 1000 ? Score > 10000 ? Globals.hoog_18 : Globals.hoog_24 : Globals.hoog_28;
            Instance.GetGui().ScoreText = $@"{scoreText}";
            Instance.GetGui().ScoreFont = font;
        }
        
        public void SetBonus(int x)
        {
            if (bonus != 0)
                return;
            bonus = x * Level;
        }

        public void Reset()
        {
            TotalLines = 0;
            Score = 0;
            Bonuses = new int[4];
            Level = SelectedLevel;
            points = 40;
            lastLevel = 0;
            bonus = 0;
        }
        
        private void CleanUp()
        {
            Remove = false;
            TotalLines += Instance.GetPlayer().GetLinesCleared(); // add one to lines removed
            Instance.GetSound().PlayClear(Instance.GetPlayer().GetLinesCleared(), WasTSpin); // play our clear sound
            if (Instance.GetPlayer().GetLinesCleared() > 0)
                leveledUp = false;
            //Little bit of math, round our total lines to the nearest tenth(Closest to zero)
            int roundedLevel = (int)Math.Round((double) TotalLines / 10, 2) * 10;
            if(!leveledUp && lastLevel != roundedLevel) {
                lastLevel = roundedLevel;
                LevelUp();
            }
            Score += points + bonus; // add 40 points to our score
            bonus = 0;
            if (TotalLines < 10)
                 Instance.GetGui().LineText = $@"00{TotalLines}";
            else if (TotalLines < 100)
                 Instance.GetGui().LineText = $@"0{TotalLines}";
            else
                 Instance.GetGui().LineText = $@"{TotalLines}";
            WasTSpin = false;
            
            Instance.GetRichPresence().UpdatePresence(); // update discord rpc to reflect new level/score.
            // GameBoard.Invalidate();
        }
        
        private void LevelUp()
        {
            if (Instance.GetGame().CurrentMode is 1 or 2)
                return;
            
            Level++;
            string addZero = Level < 10 ? "0" : "";
            Instance.GetGui().LevelText = $@"{addZero}{Level}";
            Instance.GetPlayer().SetGravity(Level);
            Instance.CurrentLevelUpImage.AnimateImage();
            if(Level != 5 && Level != 8)
                 Instance.GetSound().PlaySoundEffect("lvlup");
            else
                 Instance.GetSound().PlayWarning();
            leveledUp = true;
        }
    }
}