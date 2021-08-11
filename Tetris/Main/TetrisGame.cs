using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Tetris.GameDebug;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.Main
{
    public class TetrisGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _spriteBatchGame;
        private Viewport defaultViewport, logViewport, multViewport;
        private Texture2D logo;
        private double shakeStartAngle = 0;
        private double shakeRadius = 15;
        
        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);//809,733
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.SetUp(Content);
            _graphics.PreferredBackBufferWidth = Globals.ScreenWidth;//789
            _graphics.PreferredBackBufferHeight = Globals.ScreenHeight;//694
            _graphics.ApplyChanges();
            new Instance();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchGame = new SpriteBatch(GraphicsDevice);
            
            defaultViewport = _spriteBatchGame.GraphicsDevice.Viewport;
            logViewport = new Viewport(235,19,320,656);
            multViewport = new Viewport(793, 19, 320, 656);
            
            // TODO: use this.Content to load your game content here
            logo = Content.Load<Texture2D>("gui/tetrislogo");
            Instance.GetSound().SetUp(Content);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (Instance.InMultiplayer)//let client/server know we are disconnecting
            {
                Instance.GetPacket().SendPacketFromName("dis");
                Instance.GetPacket().RunPacketFromName("dis");
            }
            if (Server.ServerRunning() && !Instance.InMultiplayer)
            {
                Server.CloseConnection();
            }
            if (Debug.IsEnabled()) //properly close our application without error.
            {
                Debug.CloseConsole();
            }
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Globals.ResizedWindow)
            {
                _graphics.PreferredBackBufferWidth = Globals.ScreenWidth;//789
                _graphics.PreferredBackBufferHeight = Globals.ScreenHeight;//694
                _graphics.ApplyChanges();
                defaultViewport = _spriteBatchGame.GraphicsDevice.Viewport;
                logViewport = new Viewport(235,19,320,656);
                multViewport = new Viewport(793, 19, 320, 656);
                Globals.ResizedWindow = false;
            }
            
            Instance.GetKeyListener().HandleKeyPress(gameTime);

            if (!Instance.GetGame().Stopped && !Instance.GetGame().Paused)
            {
                Instance.GetPlayer().Gravity(gameTime);
                Instance.GetPlayer().Update(gameTime);
                Instance.GetPredict().BlockCollision();
                Instance.GetScoreHandler().Update();
            }

            if (Instance.InMultiplayer)
            {
                Instance.GetMultiplayerHandler().Update(gameTime);
            }

            Instance.GetGui().Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(Globals.CurrentGuiImage, new Vector2(0,0), Color.White);
            _spriteBatch.End();

            if (Instance.GetGame().CurrentScreen == 0)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(logo, new Vector2(235, 17), Color.White);
                _spriteBatch.End();
            }

            Instance.GetGui().DrawGui(_spriteBatch, gameTime);

            Vector2 offset = new Vector2(0,0);
            if (Instance.GetGame().ScreenShake)
            {
                offset = new Vector2((float)(Math.Sin(shakeStartAngle) * shakeRadius), (float)(Math.Cos(shakeStartAngle) * shakeRadius));
                shakeRadius -= 0.25f;
                shakeStartAngle += (150 + Instance.GetRandom().Next(60));
                Instance.GetPlayer().PlyY = 9999;
                if (gameTime.ElapsedGameTime.TotalSeconds - Instance.GetGame().ShakeStart > 2F || shakeRadius <= 0)
                {
                    Instance.GetPlayer().PlacedAnimation = true;
                    Instance.GetGame().ScreenShake = false;
                    shakeRadius = 15;
                    shakeStartAngle = 0;
                    Instance.GetSound().PlaySoundEffect(Instance.GetGame().Winner ? "gamewin" : "gameover");
                }
            }
            
            if (Instance.GetGame().CurrentScreen == 1 && !Instance.GetGame().Stopped)
            {
                _spriteBatchGame.GraphicsDevice.Viewport = logViewport;
                if(Instance.GetGame().ScreenShake)
                    _spriteBatchGame.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,null,null,null,null,Matrix.CreateTranslation(offset.X, offset.Y, 0));
                else
                    _spriteBatchGame.Begin();
                Instance.GetPredict().Draw(_spriteBatchGame);
                Instance.GetPlayer().DrawPlayer(_spriteBatchGame, gameTime);
                _spriteBatchGame.End();
                _spriteBatchGame.GraphicsDevice.Viewport = defaultViewport;
                Instance.GetNextShape().RenderNextShapes(_spriteBatch);
                Instance.GetHoldShape().DrawHoldShape(_spriteBatch);
            }
            _spriteBatchGame.GraphicsDevice.Viewport = logViewport;
            _spriteBatchGame.Begin();
            Animate.UpdateAnimation(_spriteBatchGame);
            _spriteBatchGame.End();
            _spriteBatchGame.GraphicsDevice.Viewport = defaultViewport;
            if (Instance.InMultiplayer)
            {
                _spriteBatchGame.GraphicsDevice.Viewport = multViewport;
                _spriteBatchGame.Begin();
                Instance.GetMultiplayerHandler().Draw(_spriteBatchGame);
                _spriteBatchGame.End();
                _spriteBatchGame.GraphicsDevice.Viewport = defaultViewport;
            }

            base.Draw(gameTime);
        }
    }
}
