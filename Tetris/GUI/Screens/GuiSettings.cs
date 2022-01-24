using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Elements;
using Tetris.Settings;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiSettings : GuiScreen
    {
        private int activeButton = -1;
        private readonly List<string> bindText = new();
        public bool CurrentlyFocused;
        private string musicSetting = "";
        private string fullscreenSetting = "";

        public override void SetUp()
        {
            base.SetUp();
            UpdateText();
            Buttons.Add(new Button(0, new Vector2(640, 130), bindText[0], Globals.Hoog24));
            Buttons.Add(new Button(1, new Vector2(640, 180), bindText[1], Globals.Hoog24));
            Buttons.Add(new Button(2, new Vector2(640, 230), bindText[2], Globals.Hoog24));
            Buttons.Add(new Button(3, new Vector2(640, 280), bindText[3], Globals.Hoog24));
            Buttons.Add(new Button(4, new Vector2(640, 330), bindText[4], Globals.Hoog24));
            Buttons.Add(new Button(5, new Vector2(640, 380), bindText[5], Globals.Hoog24));
            Buttons.Add(new Button(6, new Vector2(640, 430), bindText[6], Globals.Hoog24));
            for (var i = 0; i < 7; i++)
                Buttons[i].OnClick += BindClick;
            musicSetting = (bool)GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
            Buttons.Add(new Button(7, new Vector2(280, 130), $"Music: {musicSetting}", Globals.Hoog24));
            Buttons[7].OnClick += AudioClick;
            Buttons.Add(new Button(10, new Vector2(640, 610), "Back", Globals.Hoog48));
            Buttons[8].OnClick += o => Gui.SetCurrentScreen(new GuiMainMenu());
            fullscreenSetting = (bool)GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
            Buttons.Add(new Button(11, new Vector2(1000, 130), $"Fullscreen: {fullscreenSetting}", Globals.Hoog24));
            Buttons[9].OnClick += o =>
            {
                GameSettings.Instance
                    .ChangeToggle("Fullscreen", !(bool) GameSettings.Instance.GetOptionValue("Fullscreen"));
                fullscreenSetting = (bool)GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
                GameSettings.Instance.Save();
            };//640, 510
            Buttons.Add(new Button(12, new Vector2(1125, 425), "Reset", Globals.Hoog24));
            Buttons[10].OnClick += o =>
            {
                if(!(bool)GameSettings.Instance.GetOptionValue("Music"))
                    Sfx.Instance.PlayBackground();
                GameSettings.Instance.Reset();
                UpdateText();
                for(int i = 0; i < 6; i++)
                {
                    Buttons[i].Text = bindText[Buttons[i].Id];
                }
                Sfx.Instance.SetVolume();
                musicSetting = (bool)GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
                Buttons[7].Text = $"Music: {musicSetting}";
            };
            Buttons.Add(new Button(13, new Vector2(640, 510),"Color Editor", Globals.Hoog48));
            Buttons[11].OnClick += o => Gui.Instance.SetCurrentScreen(new GuiEditColor()); 
            Sliders.Add(new Slider((float)GameSettings.Instance.GetOptionValue("Volume"), "Volume", 182,180));
            Sliders[0].OnRelease += SetVolume;
            ButtonsDrawn = true;
        }

        private void UpdateText()
        {
            bindText.Clear();
            bindText.Add($"Left: {GameSettings.Instance.GetOptionValue("Left")}");
            bindText.Add($"Right: {GameSettings.Instance.GetOptionValue("Right")}");
            bindText.Add($"Down: {GameSettings.Instance.GetOptionValue("Down")}");
            bindText.Add($"Rotate Right: {GameSettings.Instance.GetOptionValue("RotateRight")}");
            bindText.Add($"Rotate Left: {GameSettings.Instance.GetOptionValue("RotateLeft")}");
            bindText.Add($"Hard Drop: {GameSettings.Instance.GetOptionValue("HardDrop")}");
            bindText.Add($"Hold: {GameSettings.Instance.GetOptionValue("Hold")}");
        }

        private bool IsButtonBound(Keys key)
        {
            foreach (var check in GameSettings.Instance.KeybindSettings) // see if pressed button is already bound
                if (check.GetValue() == key)
                    return true;

            return false;
        }
        
        private void SetVolume()
        {
            GameSettings.Instance.ChangeSlider("Volume", (int)(Sliders[0].GetValue()*100f)/100f);
            GameSettings.Instance.Save();
            Sfx.Instance.SetVolume();
        }

        private void AudioClick(object sender)
        {
            GameSettings.Instance.ChangeToggle("Music", !(bool) GameSettings.Instance.GetOptionValue("Music"));
            musicSetting = (bool) GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
            Sfx.Instance.PlayBackground();
            Buttons[7].Text = $"Music: {musicSetting}";
            GameSettings.Instance.Save();
        }

        private void BindClick(object sender)
        {
            var but = (Button) sender;

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

            foreach (var but in Buttons)
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
                    var translatedKey = $"{key}".Contains("oem", StringComparison.OrdinalIgnoreCase)
                        ? $"{key}".Remove(0, 3)
                        : $"{key}"; // makes the text look cleaner to the user
                    but.Text = $"{bindText[activeButton].Split(':')[0]}: {translatedKey}";
                    GameSettings.Instance
                        .ChangeKeybind($"{bindText[activeButton].Split(':')[0].Replace(" ", "")}", key);
                    DebugConsole.Instance.AddMessage("2");
                }

            activeButton = -1;
            EnableAllButtons();
            UpdateText();
            GameSettings.Instance.Save();
        }

        private void DisableAllButtons(Button button)
        {
            foreach (var but in Buttons)
            {
                if (but == button) continue;

                but.Enabled = false;
            }
        }

        private void EnableAllButtons()
        {
            foreach (var but in Buttons)
            {
                if (but.Enabled) continue;

                but.Enabled = true;
            }
        }

        public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.DrawScreen(spriteBatch, gameTime);

            spriteBatch.Begin();
            var mult = Opacity > 0.5f ? 0.5f : Opacity;
            spriteBatch.DrawBorderedRect(new Rectangle(80, 30, 1120, 450), PanelBackgroundCol * mult,
                PanelBorderCol * Opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog36, "Settings", new Vector2(640, 50), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, "Keybinds", new Vector2(640, 100), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, "Sound", new Vector2(280, 100), Color.White);
            spriteBatch.DrawCenteredString(Globals.Hoog24, "Video", new Vector2(1000, 100), Color.White);
            foreach(var slider in Sliders) slider.Draw(spriteBatch);
            foreach (var but in Buttons) but.Draw(spriteBatch, Color.White);
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            fullscreenSetting = (bool)GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
            Buttons[9].Text = $"Fullscreen: {fullscreenSetting}";
            foreach(var slider in Sliders) slider.Update(gameTime);
            base.Update(gameTime);
        }
    }
}