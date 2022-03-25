using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Managers;
using Tetris.GUI;
using Tetris.GUI.Screens;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.GUI.UiColor;
using Tetris.Settings;
using Tetris.Sound;
using Tetris.Util;

namespace Tetris.Game;

public class TetrisGame : Microsoft.Xna.Framework.Game
{
    private static TetrisGame _instance;

    public readonly GraphicsDeviceManager Graphics;

    public Viewport DefaultViewport;
    private SpriteBatch gameBatch;
    private SpriteBatch screenBatch;
    private RenderTarget2D target;
    private SpriteBatch targetBatch;

    private TetrisGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
        Graphics.PreferMultiSampling = false;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public static TetrisGame Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new TetrisGame();

            return result;
        }
    }

    protected override void Initialize()
    {
        Globals.Instance.SetUp(Content);
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
        GraphicsDevice.PresentationParameters.MultiSampleCount = 32;
        Graphics.PreferredBackBufferWidth = 1280;
        Graphics.PreferredBackBufferHeight = 720;
        Graphics.HardwareModeSwitch = false;
        Graphics.ApplyChanges();
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
        var fullscreen = (bool) GameSettings.Instance.GetOptionValue("Fullscreen");

        var bufferWidth = fullscreen ? Graphics.GraphicsDevice.DisplayMode.Width : Window.ClientBounds.Width;
        var bufferHeight = fullscreen ? Graphics.GraphicsDevice.DisplayMode.Height : Window.ClientBounds.Height;

        Window.ClientSizeChanged -= OnResize;
        Graphics.PreferredBackBufferWidth = bufferWidth;
        Graphics.PreferredBackBufferHeight = bufferHeight;
        Globals.ScreenWidth = bufferWidth;
        Globals.ScreenHeight = bufferHeight;
        Graphics.ApplyChanges();
        Window.ClientSizeChanged += OnResize;
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
        //render target to back buffer(Scales previous target to current window resolution)
        //Point wrap is so far the best sampler I could find, not perfect though(especially with weird resolutions)
        targetBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
            DepthStencilState.None, RasterizerState.CullCounterClockwise);
        targetBatch.Draw(target,
            new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight),
            Color.White);
        targetBatch.End();
        base.Draw(gameTime);
    }
}