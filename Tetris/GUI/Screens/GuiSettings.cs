using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.GUI.Control.Controls;
using Tetris.Settings;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens;

public class GuiSettings : GuiScreen
{
    private readonly List<string> bindText = new();
    private int activeButton = -1;
    public bool CurrentlyFocused;
    private string fullscreenSetting = "";
    private string musicSetting = "";

    public override void SetUp()
    {
        base.SetUp();
        UpdateText();
        AddControl(new Button(new Vector2(640, 130), bindText[0], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 180), bindText[1], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 230), bindText[2], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 280), bindText[3], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 330), bindText[4], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 380), bindText[5], Fonts.Hoog24));
        AddControl(new Button(new Vector2(640, 430), bindText[6], Fonts.Hoog24));
        for (var i = 0; i < 7; i++)
            ((Button) GetControlFromType(typeof(Button), i)).OnClick += BindClick;
        musicSetting = (bool) GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
        AddControl(new Button(new Vector2(280, 130), $"Music: {musicSetting}", Fonts.Hoog24));
        ((Button) GetControlFromType(typeof(Button), 7)).OnClick += AudioClick;
        AddControl(new Button(new Vector2(640, 610), "Back", Fonts.Hoog48));
        ((Button) GetControlFromType(typeof(Button), 8)).OnClick += o => Gui.SetCurrentScreen(new GuiMainMenu());
        fullscreenSetting = (bool) GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
        AddControl(new Button(new Vector2(1000, 130), $"Fullscreen: {fullscreenSetting}", Fonts.Hoog24));
        ((Button) GetControlFromType(typeof(Button), 9)).OnClick += o =>
        {
            GameSettings.Instance
                .ChangeToggle("Fullscreen", !(bool) GameSettings.Instance.GetOptionValue("Fullscreen"));
            fullscreenSetting = (bool) GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
            GameSettings.Instance.Save();
        }; //640, 510
        AddControl(new Button(new Vector2(1125, 425), "Reset", Fonts.Hoog24));
        ((Button) GetControlFromType(typeof(Button), 10)).OnClick += o =>
        {
            if (!(bool) GameSettings.Instance.GetOptionValue("Music"))
                Sfx.Instance.PlayBackground();
            GameSettings.Instance.Reset();
            UpdateText();
            for (var i = 0; i < 6; i++)
                ((Button) GetControlFromType(typeof(Button), i)).Text =
                    bindText[((Button) GetControlFromType(typeof(Button), i)).ID];

            Sfx.Instance.SetVolume();
            musicSetting = (bool) GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
            ((Button) GetControlFromType(typeof(Button), 7)).Text = $"Music: {musicSetting}";
        };
        AddControl(new Button(new Vector2(640, 510), "Color Editor", Fonts.Hoog48));
        ((Button) GetControlFromType(typeof(Button), 11)).OnClick +=
            o => Gui.Instance.SetCurrentScreen(new GuiEditColor());
        AddControl(new Slider((float) GameSettings.Instance.GetOptionValue("Volume"), "Volume", 182, 180));
        ((Slider) GetControlFromType(typeof(Slider), 0)).OnRelease += SetVolume;
        // AddControl(new Button(new Vector2(235, 425), "Launch Installer", Globals.Hoog24));
        // ((Button) GetControlFromType(typeof(Button), 12)).OnClick += o =>
        // {
        //     TetrisGame.Instance.Exit();
        //     if (Globals.CurrentOS == "osx")
        //     {
        //         var path = Path.Combine($"{Environment.GetEnvironmentVariable("HOME")}", "Applications", "Tetris");
        //         Process.Start(
        //             new ProcessStartInfo(Path.Combine($"{path}", "Updater", "Tetris Updater.app"))
        //                 {UseShellExecute = true});
        //     }
        //     else
        //     {
        //         Process.Start(
        //             new ProcessStartInfo(Path.Combine(
        //                     Path.Combine($"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}",
        //                         "Tetris"), "Updater", "TetrisUpdater.exe"))
        //                 {UseShellExecute = true, Verb = "OPEN"});
        //     }
        // };
        //
        // if (Globals.Standalone)
        //     ((Button) GetControlFromType(typeof(Button), 12)).Enabled = false;

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
        GameSettings.Instance.ChangeSlider("Volume",
            (int) (((Slider) GetControlFromType(typeof(Slider), 0)).GetValue() * 100f) / 100f);
        GameSettings.Instance.Save();
        Sfx.Instance.SetVolume();
    }

    private void AudioClick(object sender)
    {
        GameSettings.Instance.ChangeToggle("Music", !(bool) GameSettings.Instance.GetOptionValue("Music"));
        musicSetting = (bool) GameSettings.Instance.GetOptionValue("Music") ? "On" : "Off";
        Sfx.Instance.PlayBackground();
        ((Button) GetControlFromType(typeof(Button), 7)).Text = $"Music: {musicSetting}";
        GameSettings.Instance.Save();
    }

    private void BindClick(object sender)
    {
        var but = (Button) sender;

        if (CurrentlyFocused) // allows user to cancel bind
        {
            but.Text = bindText[but.ID];
            activeButton = -1;
            CurrentlyFocused = false;
            EnableAllButtons(); // reenable other buttons
            return;
        }

        CurrentlyFocused = true;
        activeButton = but.ID;
        but.Text = $"{but.Text.Split(':')[0]}: _";
        DisableAllButtons(but);
    }

    public void KeyPressed(Keys key)
    {
        if (activeButton == -1)
            return;

        foreach (var but in Controls)
        {
            if (but.GetType() != typeof(Button))
                continue;
            if (but.ID == activeButton)
            {
                if (IsButtonBound(key)) // if the button is already bound we cancel
                {
                    but.Text = bindText[but.ID];
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
            }
        }

        activeButton = -1;
        EnableAllButtons();
        UpdateText();
        GameSettings.Instance.Save();
    }

    private void DisableAllButtons(Button button)
    {
        foreach (var but in Controls)
        {
            if (but.GetType() != typeof(Button))
                continue;
            if (but == button) continue;

            but.Enabled = false;
        }
    }

    private void EnableAllButtons()
    {
        foreach (var but in Controls)
        {
            if (but.GetType() != typeof(Button))
                continue;
            if (but.Enabled) continue;

            but.Enabled = true;
        }
    }

    public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
    {
        base.DrawScreen(spriteBatch, gameTime);

        var mult = Opacity > 0.5f ? 0.5f : Opacity;
        spriteBatch.DrawBorderedRect(new Rectangle(80, 30, 1120, 450), PanelBackgroundCol * mult,
            PanelBorderCol * Opacity);
        spriteBatch.DrawCenteredString(Fonts.Hoog36, "Settings", new Vector2(640, 50), Color.White);
        spriteBatch.DrawCenteredString(Fonts.Hoog24, "Keybinds", new Vector2(640, 100), Color.White);
        spriteBatch.DrawCenteredString(Fonts.Hoog24, "Sound", new Vector2(280, 100), Color.White);
        spriteBatch.DrawCenteredString(Fonts.Hoog24, "Video", new Vector2(1000, 100), Color.White);
        foreach (var con in Controls)
        {
            if (con.GetType() == typeof(Slider))
                con.Draw(spriteBatch);
            if (con.GetType() == typeof(Button))
                ((Button) con).Draw(spriteBatch, Color.White);
        }
    }

    public override void Update(GameTime gameTime)
    {
        fullscreenSetting = (bool) GameSettings.Instance.GetOptionValue("Fullscreen") ? "On" : "Off";
        ((Button) GetControlFromType(typeof(Button), 9)).Text = $"Fullscreen: {fullscreenSetting}";
        base.Update(gameTime);
    }
}