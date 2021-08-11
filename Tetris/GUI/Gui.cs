using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.GUI
{
    public class Gui
    {
        public string LevelText = "01";
        public string LineText = "000";
        public string ScoreText = "0";
        public float FadeOpacity = 0f;
        private float CopyrightTime = 50f;
        public SpriteFont ScoreFont = Globals.hoog_28;
        Viewport gridViewport = new Viewport(235,19,320,656);
        private List<Button> buttons = new();

        //TODO: Add falling tetris pieces on main menu
        
        private readonly string[] CopyrightText =
        {
            "Tetris (c) 1985~2021 Tetris Holding.", "Tetris logos, Tetris theme song and","Tetriminos are trademarks of","Tetris Holding.",
            "The Tetris trade dress is owned by","Tetris Holding.", "Licensed to The Tetris Company.", "Tetris Game Design by","Alexey Pajitnov.",
            "Tetris Logo Design by Roger Dean.", "All Rights Reserved."
        };

        public void AddMenuButtons()
        {
            ClearButtons();
            Instance.GetGame().CurrentScreen = 0;
            Instance.GetRichPresence().SetPresence(0);
            Instance.GetGame().GameOver = false;
            if (!Client.IsConnected())
            {
                buttons.Add(new Button(0, new Rectangle(280, 330, 0, 0), "START GAME", Globals.hoog_24));
                buttons.Add(new Button(1, new Rectangle(272, 380, 0, 0), "MULTIPLAYER", Globals.hoog_24));
                buttons.Add(new Button(2, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons.Add(new Button(3, new Rectangle(485, 480, 0, 0), ">", Globals.hoog_24, false));
                buttons.Add(new Button(4, new Rectangle(290, 480, 0, 0), "<", Globals.hoog_24, false));
                buttons.Add(new Button(5,new Rectangle(335,654,0,0), $"Created by Carson Kelley", Globals.hoog_12, false));
                buttons[0].OnClick += StartButtonClick;
                buttons[1].OnClick += (s) => Instance.GetGuiMultiplayer().ShowMultiplayer();
                buttons[2].OnClick += SettingsButton;
                buttons[3].OnClick += LevelRight;
                buttons[4].OnClick += LevelLeft;
                buttons[5].OnClick += (s) => Process.Start(new ProcessStartInfo("https://github.com/StrugglingDoge/Tetris-MonoGame") { UseShellExecute = true });
            }
            else
            {
                buttons.Add(new Button(1, new Rectangle(272, 380, 0, 0), "MULTIPLAYER", Globals.hoog_24));
                buttons.Add(new Button(2, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons.Add(new Button(3,new Rectangle(335,654,0,0), $"Created by Carson Kelley", Globals.hoog_12, false));
                buttons[0].OnClick += (s) => Instance.GetGuiMultiplayer().ShowMultiplayer();
                buttons[1].OnClick += SettingsButton;
                buttons[2].OnClick += (s) => Process.Start(new ProcessStartInfo("https://github.com/StrugglingDoge/Tetris-MonoGame") { UseShellExecute = true });
            }
        }

        private void SettingsButton(object sender)
        {
            Instance.GetGuiSettings().ShowSettings();
        }
        
        private void LevelLeft(object sender)
        {
            int level = Instance.GetScoreHandler().SelectedLevel;
            if (level == 1)
                level = 20;
            else
                level--;
            Instance.GetScoreHandler().SelectedLevel = level;
        }

        private void LevelRight(object sender)
        {
            int level = Instance.GetScoreHandler().SelectedLevel;
            if (level == 20)
                level = 1;
            else
                level++;
            Instance.GetScoreHandler().SelectedLevel = level;
        }
        
        public void AddGameOverButton()
        {
            ClearButtons();
            Instance.GetSound().PlayBackground();
            Instance.GetTetrisRain().FadeTime = 0f;
            Instance.GetGame().CurrentScreen = 2;
            buttons.Add(new Button(0,new Rectangle(290,410,0,0), "MAIN MENU", Globals.hoog_24));
            buttons[0].OnClick += MenuClick;
        }

        public void ClearButtons()
        {
            FadeOpacity = 0f;
            buttons.Clear();
        }

        private void MenuClick(object sender)
        {
            ClearButtons();
            AddMenuButtons();
            
            Instance.GetHoldShape().ResetHold();
            Instance.GetNextShape().ResetNext();
        }
        
        public void DrawGui(SpriteBatch _spriteBatch, GameTime gameTime)
        {
            if (FadeOpacity < 1)
            {
                FadeOpacity += 0.04f;
            }
            
            _spriteBatch.Begin();

            if (CopyrightTime <= 0.1f && CopyrightTime > 0f)
            {
                Instance.GetSound().PlayBackground();
                AddMenuButtons();
                FadeOpacity = 0;
            }

            if (CopyrightTime > 0f)
            {
                _spriteBatch.Draw(Instance.DebugBox, new Rectangle(235,19,320,656), Color.Black * 0.2f);
                for (int i = 0; i < CopyrightText.Length; i++)
                {
                    int spacing = i * 18;
                    
                    _spriteBatch.DrawCenteredString(Globals.hoog_12, CopyrightText[i], new Vector2(Globals.ScreenWidth / 2, (Globals.ScreenHeight / 2) - 100 + spacing), CopyrightTime < 1f ? Color.White * CopyrightTime : Color.White);
                }
                CopyrightTime -= 0.1f;
                _spriteBatch.End();
                return;
            }
            
            if (Instance.GetGame().CurrentScreen != 1)
            {
                _spriteBatch.End();
                var oldViewport = _spriteBatch.GraphicsDevice.Viewport;
                _spriteBatch.GraphicsDevice.Viewport = gridViewport;
                _spriteBatch.Begin();
                Instance.GetTetrisRain().DrawRain(_spriteBatch, gameTime);
                _spriteBatch.End();
                _spriteBatch.GraphicsDevice.Viewport = oldViewport;
                _spriteBatch.Begin();
                
                _spriteBatch.Draw(Instance.DebugBox, new Rectangle(235,19,320,656), Color.Black * 0.2f);
            }


            Instance.GetGuiDebug().DrawDebugConsole(_spriteBatch);
            
            foreach (var but in buttons)
            {
                but.Draw(_spriteBatch, Color.White);
            }

            if (Instance.GetGame().CurrentScreen == 3)
            {
                Instance.GetGuiSettings().Draw(_spriteBatch);
            }

            if (Instance.GetGame().CurrentScreen == 4)
            {
                Instance.GetGuiMultiplayer().Draw(_spriteBatch, gameTime);
            }
            
            _spriteBatch.End();

            if (Instance.GetGame().CurrentScreen == 0)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(Globals.Logo, new Vector2(235, 17), Color.White * FadeOpacity);
                if(!Client.IsConnected())
                    _spriteBatch.DrawCenteredString(Globals.hoog_24, $"Level {Instance.GetScoreHandler().SelectedLevel}", new Vector2(395,497), Color.White * FadeOpacity);

                _spriteBatch.DrawCenteredString(Globals.hoog_12, $"{Globals.Version}", new Vector2(250,665), Color.Gray * FadeOpacity);
                _spriteBatch.End();
            }

            if (Instance.GetGame().CurrentScreen == 1)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawCenteredString(Globals.hoog_28, $"{LevelText}", new Vector2(115,275), Color.White);
                _spriteBatch.DrawCenteredString(Globals.hoog_28, $"{LineText}", new Vector2(115,405), Color.White);
                _spriteBatch.DrawCenteredString(ScoreFont, $"{ScoreText}", new Vector2(117, 535), Color.White);
                _spriteBatch.End();
            }
            
            if (Instance.GetGame().CurrentScreen == 2)
            {
                _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(235, 19, 0)));
                _spriteBatch.Draw(Globals.Stats, new Vector2(15, 172), Color.White * 0.75f);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_38, @"Game Over!", new Vector2(4,100), Color.White);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"Game Stats:", new Vector2(80,175), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"Score: {Instance.GetScoreHandler().Score}", new Vector2(35,200), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"Total Lines: {Instance.GetScoreHandler().TotalLines}", new Vector2(35,230), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"T-Spin Singles: {Instance.GetScoreHandler().Bonuses[0]}", new Vector2(35,260), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"T-Spin Doubles: {Instance.GetScoreHandler().Bonuses[1]}", new Vector2(35,290), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"T-Spin Triples: {Instance.GetScoreHandler().Bonuses[2]}", new Vector2(35,320), Color.LightGray);
                _spriteBatch.DrawStringWithShadow(Globals.hoog_18, $@"Tetrises: {Instance.GetScoreHandler().Bonuses[3]}", new Vector2(35,350), Color.LightGray);
                _spriteBatch.End();
            }
        }

        public void Update()
        {
            try
            {
                if (Instance.GetGame().CurrentScreen == 3)
                {
                    Instance.GetGuiSettings().Update();
                }
                
                if (Instance.GetGame().CurrentScreen == 4)
                {
                    Instance.GetGuiMultiplayer().Update();
                }
                
                foreach (var but in buttons)
                {
                    but.Update();
                }
                
                Instance.GetGuiDebug().Update();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        private void StartButtonClick(object sender)
        {
            Instance.GetSound().PlayBackground();
            FadeOpacity = 0f;
            Instance.GetGame().StartCountdown();
            Instance.GetPacket().SendPacketFromName("str");
        }
    }
}