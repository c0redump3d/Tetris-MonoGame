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
        private int waitTime = 5;

        public Rectangle Rec { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public bool Hovering { get; private set; }
        private bool Indicator { get; set; }
        public bool Clickable { get; set; } = true;
        public int Id { get; set; }

        public Button(int id, Rectangle rec, string text, SpriteFont font, bool showIndicator = true)
        {
            this.Id = id;
            this.Rec = rec;
            this.Text = text;
            this.Font = font;
            this.Indicator = showIndicator;
            waitTime = 5;
        }
        
        public void Draw(SpriteBatch _spriteBatch, Color col)
        {
            string tempText = $">{Text}";
            _spriteBatch.DrawString(Font, Hovering && Indicator ? tempText : Text, new Vector2(Rec.X, Rec.Y), Clickable ? col : Color.Gray);
        }
        
        public void Update(Rectangle rect) { // called every tick
            if (!Clickable)
                return;
            if (waitTime != 0)
            {
                waitTime--;
                return;
            }
            var mouseState = Mouse.GetState();
            Rec = rect;
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