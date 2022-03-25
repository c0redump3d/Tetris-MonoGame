using System;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.Game.Mode;
using Tetris.Game.Player;
using Tetris.GUI;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game.InGame;

/// <summary>
///     Manages the in-game scores like points, level, lines, and any bonuses(multiple line clears, t-spins, etc.)
/// </summary>
public class ScoreHandler
{
    private static ScoreHandler _instance;
    private int bonus;
    public int[] Bonuses = new int[4];
    private int lastLevel = 1;
    public int Level = 1;
    private bool leveledUp;

    public string LevelText = "01";
    public string LineText = "000";
    private int points = 40;
    public bool Remove;
    public int Score;
    public SpriteFont ScoreFont = Fonts.Hoog28;
    public string ScoreText = "0";
    public int SelectedLevel = 1;
    public int TotalLines;

    public bool WasTSpin { get; set; }

    private ScoreHandler()
    {
        EventManager.Instance.CreateEvent("score", CleanUp);
        EventManager.Instance.CreateEvent("levelup", LevelUp);
    }
    
    public static ScoreHandler Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new ScoreHandler();

            return result;
        }
    }

    public void Update()
    {
        if (Remove)
            EventManager.Instance.GetEvent("score").Call();
        var scoreText = Score > 999999 ? "999,999" : $"{Score:n0}";
        var font = Score > 1000 ? Score > 10000 ? Fonts.Hoog18 : Fonts.Hoog24 : Fonts.Hoog28;
        ScoreText = $@"{scoreText}";
        ScoreFont = font;
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
        TotalLines += PlayerController.Instance.GetLinesCleared(); // add one to lines removed
        Sfx.Instance.PlayClear(PlayerController.Instance.GetLinesCleared(), WasTSpin); // play our clear sound
        if (PlayerController.Instance.GetLinesCleared() > 0)
            leveledUp = false;
        //Little bit of math, round our total lines to the nearest tenth(Closest to zero)
        var roundedLevel = (int) Math.Round((double) TotalLines / 10, 2) * 10;
        if (!leveledUp && lastLevel != roundedLevel)
        {
            lastLevel = roundedLevel;
            EventManager.Instance.GetEvent("levelup").Call();
        }

        Score += points + bonus; // add 40 points to our score
        bonus = 0;
        if (TotalLines < 10)
            LineText = $@"00{TotalLines}";
        else if (TotalLines < 100)
            LineText = $@"0{TotalLines}";
        else
            LineText = $@"{TotalLines}";
        WasTSpin = false;

        RichPresence.Instance.UpdatePresence(); // update discord rpc to reflect new level/score.
    }

    private void LevelUp()
    {
        Level++;
        var addZero = Level < 10 ? "0" : "";
        LevelText = $@"{addZero}{Level}";
        PlayerController.Instance.SetGravity(Level);
        Gui.Instance.AnimateImage(Globals.CurrentLevelUpImage);
        if (Level != 5 && Level != 8)
        {
            Sfx.Instance.PlaySoundEffect("lvlup");
        }
        else
        {
            Sfx.Instance.StopMusic();
            Sfx.Instance.PlayMusic(Level);
        }

        leveledUp = true;
    }
}