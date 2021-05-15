using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tetris.GUI
{
    public class Button
    {
        public event Action<object> OnClick;
        private MouseState oldState;
        private int waitTime = 10;

        public Rectangle Rec { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public bool Hovering { get; private set; }
        private bool Indicator { get; set; }
        public bool Enabled { get; set; } = true;
        public int Id { get; set; }

        public Button(int id, Rectangle rec, string text, SpriteFont font, bool showIndicator = true)
        {
            this.Id = id;
            this.Rec = rec;
            this.Text = text;
            this.Font = font;
            this.Indicator = showIndicator;
            waitTime = 10;
        }
        
        public void Draw(SpriteBatch _spriteBatch, Color col)
        {
            string tempText = $">{Text}";
            _spriteBatch.DrawString(Font, Hovering && Indicator ? tempText : Text, new Vector2(Rec.X, Rec.Y), Enabled ? col : Color.Gray);
        }
        
        public void Update() { // called every tick
            if (!Enabled)
            {
                Hovering = false;
                return;
            }

            if (waitTime != 0)
            {
                waitTime--;
                return;
            }
            var mouseState = Mouse.GetState();
            Rec = new Rectangle(this.Rec.X, this.Rec.Y, (int) this.Font.MeasureString(this.Text).X,
                (int) this.Font.MeasureString(this.Text).Y);
            if (Rec.Contains(new Point(mouseState.X, mouseState.Y)))
            {
                Hovering = true;
                if (this.OnClick != null && mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
                {
                    // Someone's listening, and we have a click
                    this.OnClick.Invoke(this);
                }
            }
            else
            {
                Hovering = false;
            }

            this.oldState = mouseState;
        }
    }
}