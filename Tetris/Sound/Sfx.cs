using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Tetris.Other;
using Tetris.Settings;

namespace Tetris.Sound
{
    public class Sfx
    {
        private SoundEffectInstance mainTheme;
        private SoundEffectInstance fastTheme;
        private SoundEffectInstance rushTheme;
        private SoundEffectInstance pinch;
        private List<SoundEffect> soundEffects = new();
        
        public void SetUp(ContentManager content)
        {
            mainTheme = content.Load<SoundEffect>("music/musicmain").CreateInstance();
            fastTheme = content.Load<SoundEffect>("music/musiclvl5").CreateInstance();
            rushTheme = content.Load<SoundEffect>("music/musiclvl8").CreateInstance();
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
            SetVolume();
        }

        public void SetVolume()
        {
            SoundEffect.MasterVolume = AudioSettings.VOL / 100F;
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
            mainTheme.Stop();
            fastTheme.Stop();
            rushTheme.Stop();
        }
        
        public void PlayMusic(int level)
        {
            if (AudioSettings.VOL == 0 || AudioSettings.MUSIC == 0)
                return;
            if (level < 5)
            {
                mainTheme.IsLooped = true;
                mainTheme.Play();
            }
            else if (level < 8)
            {
                fastTheme.IsLooped = true;
                fastTheme.Play();
            }
            else
            {
                rushTheme.IsLooped = true;
                rushTheme.Play();
            }
        }

        public void PlayWarning()
        {
            if(Instance.GetScoreHandler().Level == 5)
            {
                PlaySoundEffect("warning5");
                Instance.CurrentLevelUpImage = Globals.levelUpTextures[1];
                Animate.StartLevelWarn();
            }
            else if(Instance.GetScoreHandler().Level == 8)
            {
                PlaySoundEffect("warning8");
                Instance.CurrentLevelUpImage = Globals.levelUpTextures[2];
                Animate.StartLevelWarn();
            }
        }
        
        public void PlayClear(int line, bool tspin)
        {
            if (line == 2)
            {
                if(!tspin)
                    soundEffects[8].Play();
                else
                    PlaySoundEffect("tspin2");
                if(!Animate.CurrentlyAnimating())
                    Globals.scoreTextures[0].AnimateImage(new Vector2(15, 300));
                Instance.GetScoreHandler().SetBonus(300);
                Instance.GetPacket().SendPacketFromName("sdb", "1");
            }
            else if (line == 3)
            {
                if(!tspin)
                    soundEffects[10].Play();
                else
                    PlaySoundEffect("tspin3");
                if(!Animate.CurrentlyAnimating())
                    Globals.scoreTextures[1].AnimateImage(new Vector2(33, 300));
                Instance.GetScoreHandler().SetBonus(500);
                Instance.GetPacket().SendPacketFromName("sdb", "2");
            }
            else if (line >= 4)
            {
                soundEffects[9].Play();
                if(!Animate.CurrentlyAnimating())
                    Globals.scoreTextures[2].AnimateImage(new Vector2(-5, 300));
                Instance.GetScoreHandler().SetBonus(800);
                Instance.GetScoreHandler().Bonuses[3]++;
                Instance.GetPacket().SendPacketFromName("sdb", "3");
            }
            else
            {
                if(!tspin)
                    soundEffects[0].CreateInstance().Play();
                else
                    PlaySoundEffect("tspin1");
            }
            soundEffects[2].CreateInstance().Play();
        }

        public void PlaySoundEffect(string name)
        {
            foreach (SoundEffect sound in soundEffects)
            {
                if (sound.Name.Split('/')[1].Equals(name, StringComparison.OrdinalIgnoreCase))
                    sound.Play();
            }
        }
        
    }
}