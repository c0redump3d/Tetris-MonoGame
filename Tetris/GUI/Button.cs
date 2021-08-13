using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Other;
using static Microsoft.Xna.Framework.Input.ButtonState;

namespace Tetris.GUI
{
    public class Button
    {
        public event Action<object> OnClick;
        private MouseState oldState;

        private Color ButtonBackCol = new Color(30, 28, 28);
        private Color ButtonBorderCol = new Color(43, 149, 223);

        public Rectangle Rec { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public bool Hovering { get; private set; }
        public bool Enabled { get; set; } = true;
        public int Size { get; set; } = 1;
        public int Id { get; set; }
        private float FadeOpacity { get; set; } = 0;
        public bool ShowBox { get; set; } = true;

        public Button(int id, Rectangle rec, string text, SpriteFont font, bool showBox = true)
        {
            this.Id = id;
            this.Rec = rec;
            this.Text = text;
            this.Font = font;
            this.ShowBox = showBox;
        }
        
        public void Draw(SpriteBatch _spriteBatch, Color col)
        {
            
            if (FadeOpacity < 1)
            {
                FadeOpacity += 0.04f;
            }

            float mult = FadeOpacity > 0.5f ? 0.5f : FadeOpacity - 0.5f;
            
            if(FadeOpacity >= 0.4f && ShowBox)
                _spriteBatch.DrawBorderedRect(new Rectangle((Rec.X - 4) - Size, Rec.Y - Size, (Rec.Width + 6) + Size * 2, Rec.Height + Size * 2), ButtonBackCol * mult, ButtonBorderCol * FadeOpacity);
            
            _spriteBatch.DrawStringWithShadow(Font, Text, new Vector2(Rec.X, Rec.Y), Enabled ? Hovering ? Color.SlateGray * FadeOpacity : col * FadeOpacity : Color.Gray);
            
            if (Instance.GetGuiDebug().IsOptionEnabled(4))
            {
                var mouseState = Mouse.GetState();
                if (Rec.Contains(mouseState.Position))
                {
                    string text =
                        $"Button ID:{this.Id} X:{Rec.X} Y:{Rec.Y}\nEnabled:{Enabled} WxH:{Rec.Width}x{Rec.Height}";
                    Rectangle debugRect =
                        new(mouseState.X - 32, mouseState.Y - 32, (int) Globals.ConsoleFont.MeasureString(text)
                            .X + 4, (int) Globals.ConsoleFont.MeasureString(text).Y);
                    _spriteBatch.Draw(Instance.DebugBox, debugRect, Color.Black * 0.5f);
                    _spriteBatch.DrawString(Globals.ConsoleFont, $"{text}",
                        new Vector2(mouseState.X - 30, mouseState.Y - 30), Color.White);
                }
            }
        }
        
        public void Update() { // called every tick
            
            var mouseState = Mouse.GetState();
            Rec = new Rectangle(this.Rec.X, this.Rec.Y, (int) this.Font.MeasureString(this.Text).X,
                (int) this.Font.MeasureString(this.Text).Y);
            if (Rec.Contains(new Point(mouseState.X, mouseState.Y)) && Enabled)
            {
                if(!Hovering)
                    Instance.GetSound().PlaySoundEffect("cursorhover");
                
                Hovering = true;

                if (Size != 3) // used to scale the button's box
                {
                    Size += 1;
                }
                
                if (this.OnClick != null && mouseState.LeftButton == Released &&
                    oldState.LeftButton == Pressed)
                {
                    // Someone's listening, and we have a click
                    Instance.GetSound().PlaySoundEffect("click");
                    this.OnClick.Invoke(this);
                }
            }
            else
            {
                if (Size != 0)
                {
                    Size -= 1;
                }
                
                Hovering = false;
            }

            this.oldState = mouseState;
        }
    }
}