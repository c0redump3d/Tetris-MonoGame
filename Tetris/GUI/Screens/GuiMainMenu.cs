using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.GUI.Elements;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.Multiplayer.Network;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.GUI.Screens
{
    public class GuiMainMenu : GuiScreen
    {
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
            Buttons.Add(new Button(0, new Vector2(1000, 170), "Play Game", Globals.Hoog48));
            Buttons.Add(new Button(1, new Vector2(1000, 270), "Multiplayer", Globals.Hoog48));
            Buttons.Add(new Button(2, new Vector2(1000, 370), "Settings", Globals.Hoog48));
            #if !__IOS__
            Buttons.Add(new Button(3, new Vector2(1000, 470), "Quit Games", Globals.Hoog48));
            #endif
            Buttons.Add(new Button(8, new Vector2(1170, 700), "Created by Carson Kelley", Globals.Hoog12, false));
            Buttons[0].OnClick += s => Gui.SetCurrentScreen(new GuiGamePreferences());
            Buttons[1].OnClick += s => Gui.SetCurrentScreen(new GuiMultiplayer());
            Buttons[2].OnClick += SettingsButton;
            #if !__IOS__
            Buttons[3].OnClick += s => TetrisGame.Instance.Exit();
            #endif
            Buttons[Buttons.Count-1].OnClick += s =>
                Process.Start(new ProcessStartInfo("https://github.com/StrugglingDoge/Tetris-MonoGame")
                    {UseShellExecute = true});
            if (NetworkManager.Instance.Connected && !NetworkManager.Instance.IsServer())
                Buttons[0].Enabled = false;
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
                spriteBatch.Begin();
                TetrisRain.Instance.DrawRain(spriteBatch, gameTime);
                spriteBatch.Draw(Globals.TexBox, new Rectangle(0, 0, 1280, 720), Color.Black * 0.2f);
                for (var i = 0; i < CopyrightText.Length; i++)
                {
                    var spacing = i * 40;

                    spriteBatch.DrawCenteredString(Globals.Hoog24, CopyrightText[i], new Vector2(640, 200 + spacing),
                        CopyrightTime < 1f ? Color.White * CopyrightTime : Color.White);
                }

                CopyrightTime -= 0.1f;
                spriteBatch.End();
                return;
            }

            base.DrawScreen(spriteBatch, gameTime);

            spriteBatch.Begin();
            spriteBatch.Draw(Globals.Logo, new Vector2(185, 175), Color.White * Opacity);

            spriteBatch.DrawString(Globals.Hoog12, $"{Globals.Version}", new Vector2(1, 700), Color.Gray * Opacity);
            
            spriteBatch.End();
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
}