using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.GameDebug;

namespace Tetris.GUI
{
    public class Button
    {
        public event Action<object> OnClick;
        private MouseState oldState;
<<<<<<< Updated upstream
        private int waitTime = 10;
=======

        private Color ButtonBackCol = new Color(30, 28, 28);
        private Color ButtonBorderCol = new Color(43, 149, 223);
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
            this.Indicator = showIndicator;
            waitTime = 10;
=======
            this.ShowBox = showBox;
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
            if (waitTime != 0)
=======
            float mult = FadeOpacity > 0.5f ? 0.5f : FadeOpacity - 0.5f;
            
            if(FadeOpacity >= 0.4f && ShowBox)
                _spriteBatch.DrawBorderedRect(new Rectangle((Rec.X - 4) - Size, Rec.Y - Size, (Rec.Width + 6) + Size * 2, Rec.Height + Size * 2), ButtonBackCol * mult, ButtonBorderCol * FadeOpacity);
            
            _spriteBatch.DrawStringWithShadow(Font, Text, new Vector2(Rec.X, Rec.Y), Enabled ? Hovering ? Color.SlateGray * FadeOpacity : col * FadeOpacity : Color.Gray);
            
            if (Instance.GetGuiDebug().IsOptionEnabled(4))
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                if (this.OnClick != null && mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
=======

                if (Size != 3) // used to scale the button's box
                {
                    Size += 1;
                }
                
                if (this.OnClick != null && mouseState.LeftButton == Released &&
                    oldState.LeftButton == Pressed)
>>>>>>> Stashed changes
                {
                    // Someone's listening, and we have a click
                    Debug.DebugMessage($"Button \"{this.Text}\"(id:{this.Id},x:{this.Rec.X},y:{this.Rec.Y}) has been clicked.", 1);
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