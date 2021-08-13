using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.Main
{
    public class TetrisGame : Game
    {
    
        //TODO: Rewrite gui stuff, make it cleaner
        //TODO: Change top out logic(Check to see if a full actual shape has locked off-screen)
    
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _spriteBatchGame;
        private Viewport defaultViewport, logViewport, multViewport;
        private MouseState oldMouseState;
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
            Instance.DebugBox = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            Instance.DebugBox.SetData(new Color[] {Color.Gray});
            Window.Title = $"Tetris ({Globals.Version})";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchGame = new SpriteBatch(GraphicsDevice);
            
            defaultViewport = _spriteBatchGame.GraphicsDevice.Viewport;
            logViewport = new Viewport(235,19,320,656);
            multViewport = new Viewport(793, 19, 320, 656);
            
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

            Instance.GetRichPresence().Shutdown();
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

            Instance.GetGui().Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(Globals.CurrentGuiImage, new Vector2(0,0), Color.White);
            _spriteBatch.End();

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
                Instance.GetPlayer().DrawPlayer(_spriteBatchGame);
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

            if (Instance.GetGame().Paused && !Instance.GetGame().IsCountdown && Instance.GetGame().CurrentScreen == 1)
            {
                _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(235, 19, 0)));
                _spriteBatch.DrawStringWithShadow(Globals.hoog_38, @"PAUSED", new Vector2(50,290), Color.White);
                _spriteBatch.End();
            }
            
            _spriteBatch.Begin();
            Instance.GetGuiDebug().DrawDebugMenu(_spriteBatch, gameTime);
            _spriteBatch.End();
            
            if (Instance.GetGuiDebug().IsOptionEnabled(5) && !Instance.GetGame().Stopped)
            {
                Rectangle[] placedRect = Instance.GetPlayer().PlacedRect;
                var mouseState = Mouse.GetState();
                Rectangle translatedMouse = new Rectangle((mouseState.X - logViewport.X),
                    (mouseState.Y - logViewport.Y),
                    10, 10); // translate the mouse position to the gameboards viewport
                
                for (int i = 0; i < placedRect.Length; i++)
                {
                    if (translatedMouse.X < placedRect[i].Right && translatedMouse.X > placedRect[i].Left &&
                        translatedMouse.Y > placedRect[i].Top && translatedMouse.Y < placedRect[i].Bottom) // rect.contain was not working for some reason
                    {
                        string text =
                            $"PlacedRect:{i} X:{placedRect[i].X} Y:{placedRect[i].Y}\nTexture:{Instance.GetPlayer().StoredImage[i].Name.Replace("blocks/tetris_", "")}";
                        Rectangle debugRect =
                            new(mouseState.X - 32, mouseState.Y - 32, (int) Globals.ConsoleFont.MeasureString(text)
                                .X + 4, (int) Globals.ConsoleFont.MeasureString(text).Y);
                        _spriteBatch.Begin(); // Draw info above mouse
                        _spriteBatch.Draw(Instance.DebugBox, debugRect, Color.Black * 0.5f);
                        _spriteBatch.DrawString(Globals.ConsoleFont, $"{text}",
                            new Vector2(mouseState.X - 30, mouseState.Y - 30), Color.White);
                        _spriteBatch.End();

                        if (mouseState.LeftButton == ButtonState.Released &&
                            oldMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Instance.GetPlayer().PlacedRect.RemoveAt(i);
                        }
                    }
                }
                
                if (mouseState.RightButton == ButtonState.Released &&
                    oldMouseState.RightButton == ButtonState.Pressed)
                {
                    translatedMouse.Y -= 16;
                    bool failed = false;
                    int boardMouseX = (int)(Math.Ceiling((double)translatedMouse.X / 32) * 32) - 32;
                    int boardMouseY = (int)(Math.Ceiling((double)translatedMouse.Y / 32) * 32);
                    for(int i = 0; i < placedRect.Length; i++) // make sure block is in-bounds and isn't going to be inside another block
                        if (placedRect[i].Contains(new Point(boardMouseX, boardMouseY)) || boardMouseY > Globals.MaxY+32 || boardMouseY < Globals.LowestY+32 || boardMouseX is < 0 or > 288)
                        {
                            failed = true;
                            break;
                        }
                    if(!failed)
                        Instance.GetPlayer().PlacedRect.Add(new Rectangle(boardMouseX, boardMouseY - 16,32,32), Globals.BlockPlacedTexture[7]);
                }
                oldMouseState = mouseState;
            }
            base.Draw(gameTime);
        }
    }
}
