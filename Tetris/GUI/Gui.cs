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
                buttons.Add(new Button(0, new Rectangle(280, 380, 0, 0), "START GAME", Globals.hoog_24));
                buttons.Add(new Button(1, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons.Add(new Button(2, new Rectangle(485, 480, 0, 0), ">", Globals.hoog_24, false));
                buttons.Add(new Button(3, new Rectangle(290, 480, 0, 0), "<", Globals.hoog_24, false));
                buttons[0].OnClick += StartButtonClick;
                buttons[1].OnClick += SettingsButton;
                buttons[2].OnClick += LevelRight;
                buttons[3].OnClick += LevelLeft;
            }
            else
            {
                buttons.Add(new Button(1, new Rectangle(310, 430, 0, 0), "SETTINGS", Globals.hoog_24));
                buttons[0].OnClick += SettingsButton;
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
            
            _spriteBatch.End();

            if (Instance.GetGame().CurrentScreen == 0 && !Client.IsConnected())
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(Globals.hoog_24, $"Level {Instance.GetScoreHandler().SelectedLevel}", new Vector2(Instance.GetScoreHandler().SelectedLevel == 1 ? 340 : 330,480), Color.White);
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
                _spriteBatch.DrawString(Globals.hoog_18, $@"Game Stats:", new Vector2(80,175), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Score: {Instance.GetScoreHandler().Score}", new Vector2(35,200), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Total Lines: {Instance.GetScoreHandler().TotalLines}", new Vector2(35,230), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Singles: {Instance.GetScoreHandler().Bonuses[0]}", new Vector2(35,260), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Doubles: {Instance.GetScoreHandler().Bonuses[1]}", new Vector2(35,290), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"T-Spin Triples: {Instance.GetScoreHandler().Bonuses[2]}", new Vector2(35,320), Color.White);
                _spriteBatch.DrawString(Globals.hoog_18, $@"Tetrises: {Instance.GetScoreHandler().Bonuses[3]}", new Vector2(35,350), Color.White);
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
                
                foreach (var but in buttons)
                {
                    but.Update(new Rectangle(but.Rec.X, but.Rec.Y, (int) but.Font.MeasureString(but.Text).X,
                        (int) but.Font.MeasureString(but.Text).Y));
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