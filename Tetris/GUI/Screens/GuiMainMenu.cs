using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.GUI.Control.Controls;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens;

public class GuiMainMenu : GuiScreen
{
    //TODO: Move copyright inforation to its own GUI class.
    private readonly string[] CopyrightText =
    {
        "Tetris (c) 1985~2021 Tetris Holding.", "Tetris logos, Tetris theme song and",
        "Tetriminos are trademarks of Tetris Holding.",
        "The Tetris trade dress is owned by Tetris Holding.", "Licensed to The Tetris Company.",
        "Tetris Game Design by Alexey Pajitnov.",
        "Tetris Logo Design by Roger Dean.", "All Rights Reserved."
    };

    private float CopyrightTime = 50f;

    public override void SetUp()
    {
        base.SetUp();
        HoldShape.Instance.ResetHold();
        NextShape.Instance.ResetNext();
        RichPresence.Instance.SetPresence(0);
        InGameManager.Instance.GameOver = false;
        AddControl(new Button(new Vector2(1000, 170), "Play Game", Fonts.Hoog48));
        AddControl(new Button(new Vector2(1000, 270), "Multiplayer", Fonts.Hoog48));
        AddControl(new Button(new Vector2(1000, 370), "Settings", Fonts.Hoog48));
        AddControl(new Button(new Vector2(1000, 470), "Quit Game", Fonts.Hoog48));
        AddControl(new Button(new Vector2(1170, 700), "Created by Carson Kelley", Fonts.Hoog12, false));
        ((Button) GetControlFromType(typeof(Button), 0)).OnClick += s => Gui.SetCurrentScreen(new GuiGamePreferences());
        ((Button) GetControlFromType(typeof(Button), 1)).OnClick += s => Gui.SetCurrentScreen(new GuiMultiplayer());
        ((Button) GetControlFromType(typeof(Button), 2)).OnClick += SettingsButton;
        ((Button) GetControlFromType(typeof(Button), 3)).OnClick += s => TetrisGame.Instance.Exit();
        ((Button) GetControlFromType(typeof(Button), 4)).OnClick += s =>
            Process.Start(new ProcessStartInfo("https://github.com/StrugglingDoge/Tetris-MonoGame")
                {UseShellExecute = true});
        /* 
            TetrisGame.Instance.Exit();
            Process.Start(
                new ProcessStartInfo($"/bin/bash")
                    {UseShellExecute = true,  Arguments = $"\"{Directory.GetCurrentDirectory()}/runupdater.sh\" {Globals.Version.Remove(0,1)}"}
            );
         */
        if (NetworkManager.Instance.Connected && !NetworkManager.Instance.IsServer())
            ((Button) GetControlFromType(typeof(Button), 0)).Enabled = false;
    }

    public override void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (CopyrightTime <= 0.1f && CopyrightTime > 0f && Gui.StartUp)
        {
            Sfx.Instance.PlayBackground();
            Opacity = 0f;
            Gui.StartUp = false;
        }

        if (CopyrightTime > 0f && Gui.StartUp)
        {
            if (Opacity < 1) Opacity += 0.04f;
            TetrisRain.Instance.UpdateRain(gameTime);
            TetrisRain.Instance.DrawRain(spriteBatch, gameTime);
            spriteBatch.Draw(Globals.TexBox, new Rectangle(0, 0, 1280, 720), Color.Black * 0.2f);
            for (var i = 0; i < CopyrightText.Length; i++)
            {
                var spacing = i * 40;

                spriteBatch.DrawCenteredString(Fonts.Hoog24, CopyrightText[i], new Vector2(640, 200 + spacing),
                    CopyrightTime < 1f ? Color.White * CopyrightTime : Color.White);
            }

            CopyrightTime -= 0.1f;
            return;
        }

        base.DrawScreen(spriteBatch, gameTime);

        spriteBatch.Draw(Globals.Logo, new Vector2(185, 175), Color.White * (Opacity < 0.75f ? Opacity : 0.75f));
        spriteBatch.DrawString(Fonts.Hoog12, $"{Globals.Version}", new Vector2(1, 700), Color.Gray * Opacity);
    }

    public override void Update(GameTime gameTime)
    {
        if (CopyrightTime > 0f && Gui.StartUp)
            return;

        base.Update(gameTime);
    }

    private void SettingsButton(object sender)
    {
        Gui.SetCurrentScreen(new GuiSettings());
    }
}