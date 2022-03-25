using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.GUI;
using Tetris.GUI.Screens;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.Sound;

public class Sfx
{
    private static Sfx _instance;
    private readonly List<SoundEffect> soundEffects = new();
    private SoundEffectInstance backgroundTheme;
    private SoundEffectInstance fastThemeLoop;
    private SoundEffectInstance mainThemeLoop;
    private SoundEffectInstance mainThemeStart;
    private SoundEffectInstance pinch;
    private SoundEffectInstance rushThemeLoop;
    private SoundEffectInstance rushThemeStart;

    public static Sfx Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new Sfx();

            return result;
        }
    }

    public void SetUp(ContentManager content)
    {
        mainThemeStart = content.Load<SoundEffect>("music/musicmainstart").CreateInstance();
        mainThemeLoop = content.Load<SoundEffect>("music/musicmainloop").CreateInstance();
        fastThemeLoop = content.Load<SoundEffect>("music/musiclvl5loop").CreateInstance();
        rushThemeStart = content.Load<SoundEffect>("music/musiclvl8start").CreateInstance();
        rushThemeLoop = content.Load<SoundEffect>("music/musiclvl8loop").CreateInstance();
        backgroundTheme = content.Load<SoundEffect>("music/mainmenu").CreateInstance();
        pinch = content.Load<SoundEffect>("sfx/pinch").CreateInstance();
        pinch.IsLooped = true;
        soundEffects.Add(content.Load<SoundEffect>("sfx/clear"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/count"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/crush"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/fall"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/gameover"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/gamewin"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/harddrop"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/hold"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/linedouble"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/lineperfect"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/linetriple"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/lvlup"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/move"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/pinch"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/rotate"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/rowfall"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/warning5"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/warning8"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/holdfail"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/tspin0"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/tspin1"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/tspin2"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/tspin3"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/cursorhover"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/click"));
        soundEffects.Add(content.Load<SoundEffect>("sfx/pause"));
        SetVolume();
    }

    public void SetVolume()
    {
        if (Gui.Instance.CurrentScreen is GuiSettings)
            if (backgroundTheme.State == SoundState.Stopped && (bool) GameSettings.Instance.GetOptionValue("Music"))
            {
                backgroundTheme.IsLooped = true;
                backgroundTheme.Volume = 0.75f - (float) GameSettings.Instance.GetOptionValue("Volume") / 2;
                backgroundTheme.Play();
            }

        SoundEffect.MasterVolume = (float) GameSettings.Instance.GetOptionValue("Volume");
    }

    public void PlayPinch()
    {
        if (pinch.State == SoundState.Playing)
            return;

        pinch.Play();
    }

    public void StopPinch()
    {
        if (pinch.State == SoundState.Stopped)
            return;

        pinch.Stop();
    }

    public void StopMusic()
    {
        mainThemeStart.Stop();
        mainThemeLoop.Stop();
        fastThemeLoop.Stop();
        rushThemeStart.Stop();
        rushThemeLoop.Stop();
    }

    public void PlayPauseMusic(int level)
    {
        if (!(bool) GameSettings.Instance.GetOptionValue("Music"))
            return;
        if (level < 5)
        {
            if (mainThemeLoop.State == SoundState.Paused)
                mainThemeLoop.Resume();
            else
                mainThemeLoop.Pause();
        }
        else if (level < 8)
        {
            if (fastThemeLoop.State == SoundState.Paused)
                fastThemeLoop.Resume();
            else
                fastThemeLoop.Pause();
        }
        else
        {
            if (rushThemeLoop.State == SoundState.Paused)
                rushThemeLoop.Resume();
            else
                rushThemeLoop.Pause();
        }
    }

    public void PlayBackground()
    {
        if (backgroundTheme.State == SoundState.Stopped)
        {
            if (!(bool) GameSettings.Instance.GetOptionValue("Music"))
                return;
            var vol = (float) GameSettings.Instance.GetOptionValue("Volume");
            backgroundTheme.Volume = 0.75f - vol / 2;
            backgroundTheme.IsLooped = true;
            backgroundTheme.Play();
        }
        else if (backgroundTheme.State == SoundState.Playing)
        {
            backgroundTheme.Stop();
        }
        else if (backgroundTheme.State == SoundState.Paused)
        {
            backgroundTheme.Resume();
        }
    }

    public void PlayMusic(int level)
    {
        if (!(bool) GameSettings.Instance.GetOptionValue("Music"))
            return;
        if (level < 5)
        {
            mainThemeLoop.IsLooped = true;
            mainThemeStart.Play();
            TimerUtil.Instance.CreateTimer(800f, () => mainThemeLoop.Play());
        }
        else if (level < 8)
        {
            fastThemeLoop.IsLooped = true;
            PlayWarning();
        }
        else
        {
            rushThemeLoop.IsLooped = true;
            PlayWarning();
        }
    }

    private void PlayWarning()
    {
        StopMusic();
        if (ScoreHandler.Instance.Level is >= 5 and < 8)
        {
            PlaySoundEffect("warning5");
            TimerUtil.Instance.CreateTimer(1500f, () => fastThemeLoop.Play());
            Globals.CurrentLevelUpImage = Globals.LevelUpTextures[1];
        }
        else if (ScoreHandler.Instance.Level >= 8)
        {
            PlaySoundEffect("warning8");
            TimerUtil.Instance.CreateTimer(1500f, () =>
            {
                if (!InGameManager.Instance.Paused)
                    rushThemeStart.Play();
                TimerUtil.Instance.CreateTimer(5655f, () => rushThemeLoop.Play());
            });
            Globals.CurrentLevelUpImage = Globals.LevelUpTextures[2];
        }
    }

    public void PlayClear(int line, bool tspin)
    {
        if (line == 2)
        {
            if (!tspin)
                soundEffects[8].Play();
            else
                PlaySoundEffect("tspin2");
            Gui.Instance.AnimateImage(Globals.ScoreTextures[0]);
            ScoreHandler.Instance.SetBonus(300);
        }
        else if (line == 3)
        {
            if (!tspin)
                soundEffects[10].Play();
            else
                PlaySoundEffect("tspin3");
            Gui.Instance.AnimateImage(Globals.ScoreTextures[1]);
            ScoreHandler.Instance.SetBonus(500);
        }
        else if (line >= 4)
        {
            soundEffects[9].Play();
            Gui.Instance.AnimateImage(Globals.ScoreTextures[2]);
            ScoreHandler.Instance.SetBonus(800);
            ScoreHandler.Instance.Bonuses[3]++;
        }
        else
        {
            if (!tspin)
                soundEffects[0].CreateInstance().Play();
            else
                PlaySoundEffect("tspin1");
        }

        soundEffects[2].CreateInstance().Play();
    }

    public void PlaySoundEffect(string name)
    {
        foreach (var sound in soundEffects)
            if (sound.Name.Split('/')[1].Equals(name, StringComparison.OrdinalIgnoreCase))
                sound.Play();
    }
}