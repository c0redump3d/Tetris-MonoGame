using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Tetris.Multiplayer.Network;
using Tetris.Other;

namespace Tetris.GUI
{
    public class Gui
    {
        public string LevelText = "01";
        public string LineText = "000";
        public string ScoreText = "0";
        public SpriteFont ScoreFont = Globals.hoog_28;
        private List<Button> buttons = new();

        public Gui()
        {
            AddMenuButtons();
        }

        //TODO: Add a multiplayer button(will need a text box of some sort)
        public void AddMenuButtons()
        {
            ClearButtons();
            Instance.GetGame().CurrentScreen = 0;
            Instance.GetGame().GameOver = false;
            if (!Client.IsConnected())
            {
                buttons.Add(new Button(0, new Rectangle(280, 330, 0, 0), "START GAME", Globals.hoog_24));
                buttons.Add(new Button(1, new Rectangle(272, 380, 0, 0), "MULTIPLAYER", Globals.hoog_24));
                buttons.Add(new Button(2, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons.Add(new Button(3, new Rectangle(485, 480, 0, 0), ">", Globals.hoog_24, false));
                buttons.Add(new Button(4, new Rectangle(290, 480, 0, 0), "<", Globals.hoog_24, false));
                buttons[0].OnClick += StartButtonClick;
                buttons[1].OnClick += (s) => Instance.GetGuiMultiplayer().ShowMultiplayer();
                buttons[2].OnClick += SettingsButton;
                buttons[3].OnClick += LevelRight;
                buttons[4].OnClick += LevelLeft;
            }
            else
            {
                buttons.Add(new Button(1, new Rectangle(272, 380, 0, 0), "MULTIPLAYER", Globals.hoog_24));
                buttons.Add(new Button(2, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons[0].OnClick += (s) => Instance.GetGuiMultiplayer().ShowMultiplayer();
                buttons[1].OnClick += SettingsButton;
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
                level = 10;
            else
                level--;
            Instance.GetScoreHandler().SelectedLevel = level;
        }

        private void LevelRight(object sender)
        {
            int level = Instance.GetScoreHandler().SelectedLevel;
            if (level == 10)
                level = 1;
            else
                level++;
            Instance.GetScoreHandler().SelectedLevel = level;
        }
        
        public void AddGameOverButton()
        {
            ClearButtons();
            Instance.GetGame().CurrentScreen = 2;
            buttons.Add(new Button(0,new Rectangle(290,410,0,0), "MAIN MENU", Globals.hoog_24));
            buttons[0].OnClick += MenuClick;
        }

        public void ClearButtons()
        {
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
            _spriteBatch.Begin();

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

            if (Instance.GetGame().CurrentScreen == 0 && !Client.IsConnected())
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawCenteredString(Globals.hoog_24, $"Level {Instance.GetScoreHandler().SelectedLevel}", new Vector2(395,497), Color.White);
                _spriteBatch.DrawString(Globals.hoog_12, $"Created by Carson Kelley", new Vector2(335,655), Color.White);
                _spriteBatch.End();
            }

            if (Instance.GetGame().CurrentScreen == 1)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawCenteredString(Globals.hoog_28, $"{LevelText}", new Vector2(115,275), Color.White);
                _spriteBatch.DrawCenteredString(Globals.hoog_28, $"{LineText}", new Vector2(115,405), Color.White);
                _spriteBatch.DrawCenteredString(ScoreFont, $"{ScoreText}", new Vector2(115, 535), Color.White);
                _spriteBatch.End();
            }
            
            if (Instance.GetGame().CurrentScreen == 2)
            {
                _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(235, 19, 0)));
                _spriteBatch.Draw(Globals.Stats, new Vector2(15, 172), Color.White * 0.75f);
                _spriteBatch.DrawString(Globals.hoog_38, @"Game Over!", new Vector2(4,100), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Game Stats:", new Vector2(80,175), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Score: {Instance.GetScoreHandler().Score}", new Vector2(35,200), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Total Lines: {Instance.GetScoreHandler().TotalLines}", new Vector2(35,230), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Singles: {Instance.GetScoreHandler().Bonuses[0]}", new Vector2(35,260), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Doubles: {Instance.GetScoreHandler().Bonuses[1]}", new Vector2(35,290), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Triples: {Instance.GetScoreHandler().Bonuses[2]}", new Vector2(35,320), Color.LightGray);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Tetrises: {Instance.GetScoreHandler().Bonuses[3]}", new Vector2(35,350), Color.LightGray);
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
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        private void StartButtonClick(object sender)
        {
            Instance.GetGame().StartCountdown();
            Instance.GetPacket().SendPacketFromName("str");
        }
    }
}