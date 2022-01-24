using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.Elements;
using Tetris.GUI.Particle;
using Tetris.Util;

namespace Tetris.GUI
{
    public class Gui
    {
        /*
         *
         *  MENU 0 - GuiMainMenu
         *  MENU 1 - GuiInGame
         *  MENU 2 - GuiGameOver
         *  MENU 3 - GuiSettings
         *  MENU 4 - GuiMultiplayer
         * 
         */
        public TextBox CurrentTextBox = null;
        public GuiScreen CurrentScreen;
        //Next screen is used so that we can have a type of transition between the two menus(Fade out old menu, fade in new)
        private GuiScreen NextScreen;
        public bool StartUp = true;

        public string MultiplayerMessage = "";

        public void SetCurrentScreen(GuiScreen screen)
        {
            if (CurrentScreen != null)
            {
                NextScreen = screen;
                CurrentScreen.Closing = true;
            }
            else
            {
                CurrentScreen = screen;
                CurrentScreen.SetUp();
            }
        }

        public void DrawGui(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (CurrentScreen != null)
                CurrentScreen.DrawScreen(spriteBatch, gameTime);

            DebugMenu.DebugMenu.Instance.Draw(spriteBatch, gameTime);
            DebugConsole.Instance.DrawDebugConsole(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentScreen != null)
            {
                if (CurrentScreen.Closing && CurrentScreen.Opacity < 0f)
                {
                    CurrentScreen = NextScreen;
                    CurrentScreen.SetUp();
                }

                CurrentScreen.Update(gameTime);
            }

            ParticleManager.Instance.UpdateParticles(gameTime);

            DebugMenu.DebugMenu.Instance.Update(gameTime);
        }

        public Vector2 TranslateMousePosition(MouseState state)
        {
            var virtualX = Convert.ToSingle(state.X) * Convert.ToSingle(1280) / Convert.ToSingle(Globals.ScreenWidth);
            var virtualY = Convert.ToSingle(state.Y) * Convert.ToSingle(720) / Convert.ToSingle(Globals.ScreenHeight);
            var mousePos = new Vector2(virtualX, virtualY);

            return mousePos;
        }

        private static Gui _instance;
        public static Gui Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new Gui();
                }

                return result;
            }
        }
    }
}