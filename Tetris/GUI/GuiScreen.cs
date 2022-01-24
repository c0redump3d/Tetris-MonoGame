﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.Elements;
using Tetris.GUI.Particle;
using Tetris.GUI.Particle.Particles;
using Tetris.GUI.Screens.ScreenAnimations;
using Tetris.GUI.UiColor;
using Tetris.Multiplayer.Network;

namespace Tetris.GUI
{
    public class GuiScreen
    {
        private float particleWait = 100f;
        protected Color PanelBackgroundCol = new(30, 28, 28);
        protected Color PanelBorderCol = new(97, 21, 179);
        protected List<Button> Buttons = new();
        protected List<TextBox> TextBoxes = new();
        protected List<Slider> Sliders = new();
        protected bool ButtonsDrawn = false;
        protected bool Rain = true;
        protected Gui Gui;
        public float Opacity;
        public bool Closing = false;

        public virtual void SetUp()
        {
            Gui = Gui.Instance;
            Opacity = 0f;
            Buttons = new List<Button>();
            TextBoxes = new List<TextBox>();
            Sliders = new List<Slider>();
        }

        public virtual void DrawScreen(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ScrollingStars.Instance.DrawStars(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.Draw(Globals.TexBox, new Rectangle(0, 0, 1280, 720), Color.Black * 0.25f);
            if (Rain)
            {
                TetrisRain.Instance.DrawRain(spriteBatch, gameTime);
                spriteBatch.Draw(Globals.TexBox, new Rectangle(0, 0, 1280, 720), Color.Black * 0.15f);
                spriteBatch.End();
                ParticleManager.Instance.DrawParticles(spriteBatch);
                spriteBatch.Begin();
            }

            if (NetworkManager.Instance.Connected)
            {
                string ping = $"Ping: {NetworkManager.Instance.GetPing()}ms";
                
                spriteBatch.DrawString(Globals.Hoog16, "Connected", new Vector2(1275 - Globals.Hoog16.MeasureString("Connected").X, 0), Color.Green);
                spriteBatch.DrawString(Globals.Hoog16, ping, new Vector2(1275 - Globals.Hoog16.MeasureString(ping).X, 20), Color.White);
            }

            //incase i need to draw buttons over something
            if (!ButtonsDrawn)
                foreach (var but in Buttons)
                    but.Draw(spriteBatch, Color.White);

            spriteBatch.End();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Rain)
            {
                particleWait -= gameTime.ElapsedGameTime.Milliseconds;
                TetrisRain.Instance.UpdateRain(gameTime);
                if (particleWait < 0f)
                {
                    ParticleManager.Instance.Create(new Circle(GameManager.Instance.Random.Next(-720,720), GameManager.Instance.Random.Next(720)));
                    particleWait = 100f;
                }
            }
            ScrollingStars.Instance.UpdateStarBG();

            PanelBackgroundCol = ColorManager.Instance.GuiColor["Panel Background"];
            PanelBorderCol = ColorManager.Instance.GuiColor["Panel Border"];
            
            if (Opacity < 1 && !Closing)
                Opacity += 0.04f;
            else if (Closing && Opacity > 0) Opacity -= 0.04f;

            foreach (var but in Buttons) but.Update();
            foreach(var tbox in TextBoxes)
                if (tbox.Focused)
                    Gui.Instance.CurrentTextBox = tbox;
        }
    }
}