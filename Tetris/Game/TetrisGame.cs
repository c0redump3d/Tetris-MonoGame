using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Managers;
using Tetris.GUI;
using Tetris.GUI.Screens;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.GUI.UiColor;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game
{
    public class TetrisGame : Microsoft.Xna.Framework.Game
    {

        public readonly GraphicsDeviceManager Graphics;
        private SpriteBatch screenBatch;
        private SpriteBatch gameBatch;
        private RenderTarget2D target;
        private SpriteBatch targetBatch;

        public Viewport DefaultViewport;

        private TetrisGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.Instance.SetUp(Content);
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferMultiSampling = true;
            Graphics.HardwareModeSwitch = false;
            Graphics.ApplyChanges();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            targetBatch = new SpriteBatch(GraphicsDevice);
            target = new RenderTarget2D(GraphicsDevice, 1280, 720);
            Window.Title = $"Tetris ({Globals.Version})";

            TetrisRain.Instance.SetUp();
            ColorManager.Instance.LoadColors();
            Gui.Instance.SetCurrentScreen(new GuiMainMenu());
            base.Initialize();
        }

        protected override void LoadContent()
        {
            screenBatch = new SpriteBatch(GraphicsDevice);
            gameBatch = new SpriteBatch(GraphicsDevice);
            DefaultViewport = gameBatch.GraphicsDevice.Viewport;

            Sfx.Instance.SetUp(Content);
        }

        private void OnResize(object o, EventArgs e)
        {
            if (GameManager.Instance.IsFullscreen() || GameManager.Instance.IsFullscreening())
                return;
            Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            Globals.ScreenWidth = Window.ClientBounds.Width;
            Globals.ScreenHeight = Window.ClientBounds.Height;
            Graphics.ApplyChanges();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (GameManager.Instance.IsFullscreen())
            {
                Graphics.ToggleFullScreen();
                Graphics.ApplyChanges();
            }

            RichPresence.Instance.Shutdown();
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            GameManager.Instance.UpdateAll(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //change our render target
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw code here
            GameManager.Instance.DrawAll(gameBatch, screenBatch, gameTime);

            //set rendering back to the back buffer
            GraphicsDevice.SetRenderTarget(null);
            //render target to back buffer
            targetBatch.Begin(samplerState: SamplerState.PointClamp);
            targetBatch.Draw(target,
                new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight),
                Color.White);
            targetBatch.End();
            base.Draw(gameTime);
        }

        private static TetrisGame _instance;
        public static TetrisGame Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new TetrisGame();
                }

                return result;
            }
        }
    }
}