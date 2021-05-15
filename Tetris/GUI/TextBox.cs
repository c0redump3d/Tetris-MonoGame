using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Other;

namespace Tetris.GUI
{
    public class TextBox
    {
        //This will most likely be changed to be more modular in the future, but for now it gets the job done.
        private MouseState oldState;
        private KeyboardState oldKeyState;
        private double cursorTime = 0;
        public Rectangle Rec { get; set; }
        public bool Focused { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Text { get; set; }
        
        public TextBox(int x, int y)
        {
            this.Rec = new Rectangle(x,y,280,30);
            this.X = x;
            this.Y = y;
            Text = "192.168.1.1";
        }

        public void Update()
        {
            UpdateText();
            var mouseState = Mouse.GetState();
            if (Rec.Contains(new Point(mouseState.X, mouseState.Y)))
            {
                //Hovering = true;
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
                {
                    if (Text == "192.168.1.1")
                        Text = "";
                    // Someone's listening, and we have a click
                    Focused = true;
                }
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
                {
                    Focused = false;
                }
            }

            this.oldState = mouseState;
        }
        
        private void UpdateText()
        {
            if (!Focused)
                return;
            KeyboardState state = Keyboard.GetState();

            for (int i = 0; i < state.GetPressedKeyCount(); i++)
            {
                if (!state.IsKeyDown(state.GetPressedKeys()[i]) || oldKeyState.IsKeyDown(state.GetPressedKeys()[i])
                 || Text.Length > 14)
                    continue;
                if (isNumber(state.GetPressedKeys()[i]) || state.GetPressedKeys()[i] == Keys.Decimal || state.GetPressedKeys()[i] == Keys.OemPeriod)
                {
                    if (state.GetPressedKeys()[i] == Keys.Decimal || state.GetPressedKeys()[i] == Keys.OemPeriod)
                    {
                        Text += ".";
                        break;
                    }
                    string num = state.GetPressedKeys()[i].ToString();
                    Text += num[^1];
                }
            }

            if (state.IsKeyDown(Keys.Back) && !oldKeyState.IsKeyDown(Keys.Back))
            {
                if (Text.Length == 0)
                    return;
                Text = Text.Remove(Text.Length-1);
            }

            oldKeyState = state;
        }

        private bool isNumber(Keys key)
        {
            return key is Keys.D0 or Keys.NumPad0 or Keys.D1 or Keys.NumPad1 or Keys.D2 or Keys.NumPad2 or Keys.D3 or Keys.NumPad3
                or Keys.D4 or Keys.NumPad4 or Keys.D5 or Keys.NumPad5 or Keys.D6 or Keys.NumPad6 or Keys.D7 or Keys.NumPad7 or Keys.D8 or Keys.NumPad8 or Keys.D9 or Keys.NumPad9;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            string cursor = "";

            if (Focused && Text.Length < 15)
            {
                cursorTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (cursorTime <= 500)
                {
                    cursor = "_";
                }

                if (cursorTime <= 0)
                    cursorTime = 1000;
            }
            
            spriteBatch.Draw(Globals.TextBoxGui, Rec, Color.White * 0.8F);
            spriteBatch.DrawCenteredString(Globals.hoog_18, cursor, new Vector2(Globals.hoog_18.MeasureString(Text + cursor).X / 2 + Rec.Center.X, Rec.Center.Y), Color.LightGray);
            spriteBatch.DrawCenteredString(Globals.hoog_18, Text, new Vector2(Rec.Center.X, Rec.Center.Y), Color.LightGray);
        }
        
    }
}