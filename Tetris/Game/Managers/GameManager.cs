using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.GUI;
using Tetris.Multiplayer.Network;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.Game.Managers
{
    public class GameManager
    {
        
        public readonly Random Random = new();
        
        public void UpdateAll(GameTime gameTime)
        {
            OnKeyPress(gameTime);
            NetworkManager.Instance.UpdateNetwork(gameTime);
            TimerUtil.Instance.UpdateTimers(gameTime);
            InGameManager.Instance.UpdateGame(gameTime);
            Gui.Instance.Update(gameTime);
        }

        /// <summary>
        /// Draws the game to the window. Note: everything drawn in this field will be rendered to a render target of 1280x720
        /// </summary>
        public void DrawAll(SpriteBatch gameBatch, SpriteBatch screenBatch, GameTime gameTime)
        {
            screenBatch.Begin();
            screenBatch.Draw(Globals.GradientBackground, new Vector2(0, 0), Color.White);
            screenBatch.End();
            Gui.Instance.DrawGui(gameBatch, gameTime);
            InGameManager.Instance.DrawGame(gameBatch, screenBatch);
        }

        private KeyboardState keyState;
        private KeyboardState oldKeyState;
        private void OnKeyPress(GameTime gameTime)
        {
            keyState = Keyboard.GetState();
            
            KeyboardInput.Instance.HandleKeyPress(gameTime);
            
            if (oldKeyState.IsKeyDown(Keys.LeftAlt) && keyState.IsKeyDown(Keys.Enter) &&
                !oldKeyState.IsKeyDown(Keys.Enter) || IsFullscreen() != (bool)GameSettings.Instance.GetOptionValue("Fullscreen"))
            {
                GameSettings.Instance.ChangeToggle("Fullscreen", !IsFullscreen());
                GameSettings.Instance.Save();
                Fullscreen();
            }

            oldKeyState = keyState;
        }
        
        /// <summary>
        /// Starts the application.
        /// </summary>
        public void RunGame()
        {
            using var game = TetrisGame.Instance;
            game.Run();
        }
        
        public bool IsFullscreening() => fullscreening;
        public bool IsFullscreen() => isFullscreen;
        private int oldWidth, oldHeight;
        private bool isFullscreen;
        private bool fullscreening;
        private void Fullscreen()
        {
            var graphics = TetrisGame.Instance.Graphics;
            fullscreening = true;
            isFullscreen = (bool)GameSettings.Instance.GetOptionValue("Fullscreen");
            if (isFullscreen)
            {
                oldWidth = graphics.PreferredBackBufferWidth;
                oldHeight = graphics.PreferredBackBufferHeight;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Globals.ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Globals.ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
                graphics.PreferredBackBufferWidth = oldWidth;
                graphics.PreferredBackBufferHeight = oldHeight;
                Globals.ScreenWidth = oldWidth;
                Globals.ScreenHeight = oldHeight;
            }

            graphics.ApplyChanges();
            fullscreening = false;
        }
        
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new GameManager();
                }

                return result;
            }
        }
    }
}