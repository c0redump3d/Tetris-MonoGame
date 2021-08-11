using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Other;
using Tetris.Settings;

namespace Tetris.GUI
{
    public class GuiSettings
    {
        private List<Button> buttons = new List<Button>();
        public bool CurrentlyFocused = false;
        private int activeButton = -1;
        private string musicSetting = "";
        private List<string> bindText = new();
        private readonly Dictionary<string, Keys> keys = new Dictionary<string, Keys>();

        public void ShowSettings()
        {
            Instance.GetGame().CurrentScreen = 3;
            Instance.GetGui().ClearButtons();
            buttons.Clear();
            UpdateText();
            UpdateDictionaries();
            buttons.Add(new Button(0,new Rectangle(245,55,0,0), bindText[0], Globals.hoog_18));
            buttons.Add(new Button(1,new Rectangle(245,90,0,0), bindText[1], Globals.hoog_18));
            buttons.Add(new Button(2,new Rectangle(245,125,0,0), bindText[2], Globals.hoog_18));
            buttons.Add(new Button(3,new Rectangle(245,160,0,0), bindText[3], Globals.hoog_18));
            buttons.Add(new Button(4,new Rectangle(245,195,0,0), bindText[4], Globals.hoog_18));
            buttons.Add(new Button(5,new Rectangle(245,230,0,0), bindText[5], Globals.hoog_18));
            buttons.Add(new Button(6,new Rectangle(245,265,0,0), bindText[6], Globals.hoog_18));
            for (int i = 0; i < 7; i++)
                buttons[i].OnClick += BindClick;
            musicSetting = AudioSettings.MUSIC == 1 ? "On" : "Off";
            buttons.Add(new Button(7,new Rectangle(245,300,0,0), $"Music: {musicSetting}", Globals.hoog_18));
            buttons[7].OnClick += AudioClick;
            buttons.Add(new Button(8,new Rectangle(480,340,0,0),">", Globals.hoog_18,false));
            buttons.Add(new Button(9,new Rectangle(300,340,0,0),"<", Globals.hoog_18,false));
            buttons[8].OnClick += AudioClick;
            buttons[9].OnClick += AudioClick;
            buttons.Add(new Button(9,new Rectangle(320,385,0,0),"MAIN MENU", Globals.hoog_18));
            buttons[10].OnClick += (s) => Instance.GetGui().AddMenuButtons();
        }
        
        private void UpdateDictionaries()
        {
            keys.Clear();
            keys.Add("Left", MovementKeys.LEFT);
            keys.Add("Right", MovementKeys.RIGHT);
            keys.Add("Down", MovementKeys.DOWN);
            keys.Add("RotateRight", MovementKeys.ROTATERIGHT);
            keys.Add("RotateLeft", MovementKeys.ROTATELEFT);
            keys.Add("Forcedrop", MovementKeys.FORCEDROP);
            keys.Add("Hold", MovementKeys.HOLD);
        }
        
        private void UpdateText()
        {
            bindText.Clear();
            bindText.Add($"Left: {MovementKeys.LEFT}");
            bindText.Add($"Right: {MovementKeys.RIGHT}");
            bindText.Add($"Down: {MovementKeys.DOWN}");
            bindText.Add($"Rotate Right: {MovementKeys.ROTATERIGHT}");
            bindText.Add($"Rotate Left: {MovementKeys.ROTATELEFT}");
            bindText.Add($"Forcedrop: {MovementKeys.FORCEDROP}");
            bindText.Add($"Hold: {MovementKeys.HOLD}");
        }
        
        private bool IsButtonBound(Keys key)
        {
            foreach (Keys check in MovementKeys.CONTROLS.Values) // see if pressed button is already bound
            {
                if (check == key)
                {
                    return true;
                }
            }

            return false;
        }
        
        private void AudioClick(object sender)
        {
            Button but = ((Button) sender);

            if (but.Text.Contains("Music"))
            {
                new AudioSettings(AudioSettings.VOL, AudioSettings.MUSIC == 1 ? 0 : 1);
                musicSetting = AudioSettings.MUSIC == 1 ? "On" : "Off";
                buttons[7].Text = $"Music: {musicSetting}";
            }

            if (but.Id == 8)
            {
                if (AudioSettings.VOL == 100)
                    return;
                new AudioSettings(AudioSettings.VOL + 5, AudioSettings.MUSIC);
            }
            if (but.Id == 9)
            {
                if (AudioSettings.VOL == 0)
                    return;
                new AudioSettings(AudioSettings.VOL - 5, AudioSettings.MUSIC);
            }
            
            UpdateDictionaries();
            Instance.GetSound().SetVolume();
            Instance.GetGameSettings().SaveSettings();
        }
        
        private void BindClick(object sender)
        {
            Button but = ((Button) sender);

            if (CurrentlyFocused) // allows user to cancel bind
            {
                but.Text = bindText[but.Id];
                activeButton = -1;
                CurrentlyFocused = false;
                EnableAllButtons(); // reenable other buttons
                return;
            }

            CurrentlyFocused = true;
            activeButton = but.Id;
            but.Text = $"{but.Text.Split(':')[0]}: _";
            DisableAllButtons(but);
        }

        public void KeyPressed(Keys key)
        {
            if (activeButton == -1)
                return;
            
            foreach (var but in buttons)
            {
                if (but.Id == activeButton)
                {
                    
                    if (IsButtonBound(key)) // if the button is already bound we cancel
                    {
                        but.Text = bindText[but.Id];
                        activeButton = -1;
                        CurrentlyFocused = false;
                        EnableAllButtons();
                        return;
                    }
                    
                    CurrentlyFocused = false;
                    string translatedKey = $"{key}".Contains("oem", StringComparison.OrdinalIgnoreCase)
                        ? $"{key}".Remove(0, 3)
                        : $"{key}"; // makes the text look cleaner to the user
                    but.Text = $"{bindText[activeButton].Split(':')[0]}: {translatedKey}";
                    keys[but.Text.Split(':')[0].Replace(" ", "")] = key;
                }
            }

            activeButton = -1;
            EnableAllButtons();
            new MovementKeys(keys["Left"], keys["Right"], keys["Down"], keys["RotateRight"], keys["RotateLeft"], keys["Forcedrop"], keys["Hold"]);
            UpdateText();
            UpdateDictionaries();
            Instance.GetGameSettings().SaveSettings();
        }

        private void DisableAllButtons(Button button)
        {
            foreach (var but in buttons)
            {
                if (but == button)
                {
                    continue;
                }

                but.Enabled = false;
            }
        }
        
        private void EnableAllButtons()
        {
            foreach (var but in buttons)
            {
                if (but.Enabled)
                {
                    continue;
                }

                but.Enabled = true;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCenteredString(Globals.hoog_28, "SETTINGS", new Vector2(394,35), Color.White);
            spriteBatch.DrawCenteredString(Globals.hoog_18, $"Volume: {AudioSettings.VOL}", new Vector2(395,353), Color.White * Instance.GetGui().FadeOpacity);
            
            foreach (var but in buttons)
            {
                but.Draw(spriteBatch, Color.White);
            }
        }

        public void Update()
        {
            try
            {
                foreach (var but in buttons)
                {
                    but.Update();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}