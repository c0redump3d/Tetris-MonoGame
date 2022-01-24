using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Elements
{
    public class TextBox
    {
        private double cursorTime;

        private Color ButtonBackCol = ColorManager.Instance.GuiColor["Text Box Background"];
        private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Text Box Border"];

        //This will most likely be changed to be more modular in the future, but for now it gets the job done.
        private MouseState oldState;
        private SpriteFont font;
        private int firstClick = 0;

        public TextBox(int x, int y, string defaultText, int maxLength, string textFilter = "", int width = 500, int height = 50)
        {
            Rec = new Rectangle(x, y, width, height);
            font = width < 500 ? Globals.Hoog12 : Globals.Hoog24;
            X = x;
            Y = y;
            DefaultText = defaultText;
            Text = defaultText;
            MaxLength = maxLength;
            if (textFilter != "")
                TextFilter = textFilter;
        }

        public Rectangle Rec { get; set; }
        public bool Focused { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Text { get; set; }
        public string DefaultText { get; set; }
        public int MaxLength { get; set; }
        public string TextFilter { get; set; } = @"^[a-zA-Z0-9.\-]*?$";

        public void Update()
        {
            if (Gui.Instance.CurrentScreen.Closing)
                return;
            ButtonBackCol = ColorManager.Instance.GuiColor["Text Box Background"];
            ButtonBorderCol = ColorManager.Instance.GuiColor["Text Box Border"];
            var mouseState = Mouse.GetState();
            var mousePos = Gui.Instance.TranslateMousePosition(mouseState);
            if (Rec.Contains(new Point((int) mousePos.X, (int) mousePos.Y)))
            {
                //Hovering = true;
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
                {
                    if (Text == DefaultText && firstClick == 0)
                    {
                        Text = "";
                        firstClick++;
                    }

                    Gui.Instance.CurrentTextBox = this;
                    Focused = true;
                }
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    oldState.LeftButton == ButtonState.Released)
                {
                    Focused = false;
                    Gui.Instance.CurrentTextBox = null;
                    if (Text == "")
                    {
                        Text = DefaultText;
                        firstClick = 0;
                    }
                }
            }

            oldState = mouseState;
        }

        public void UpdateText(char key)
        {
            if (!Focused)
                return;
            var state = Keyboard.GetState();
            
            bool stringIsValid = Regex.IsMatch(key.ToString(), @$"{TextFilter}");
            
            if(Text.Length < MaxLength && stringIsValid)
                Text += key;

            if (state.IsKeyDown(Keys.Back))
            {
                if (Text.Length == 0)
                    return;
                Text = Text.Remove(Text.Length - 1);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var cursor = "";

            if (Focused && Text.Length < MaxLength)
            {
                cursorTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (cursorTime <= 500) cursor = "_";

                if (cursorTime <= 0)
                    cursorTime = 1000;
            }
            
            var opacity = Gui.Instance.CurrentScreen.Opacity;
            var mult = opacity > 0.8f ? 0.8f : opacity - 0.2f;
            
            spriteBatch.DrawBorderedRect(Rec, ButtonBackCol * mult, ButtonBorderCol * opacity);
            spriteBatch.DrawCenteredString(font, cursor,
                new Vector2(font.MeasureString(Text + cursor).X / 2 + Rec.Center.X, Rec.Center.Y),
                Color.LightGray);
            spriteBatch.DrawCenteredString(font, Text, new Vector2(Rec.Center.X, Rec.Center.Y),
                Color.LightGray);
        }
    }
}