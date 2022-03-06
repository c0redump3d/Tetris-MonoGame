using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls
{
    public class TextBox : UiControl
    {
        private double cursorTime;

        private Color ButtonBackCol = ColorManager.Instance.GuiColor["Text Box Background"];
        private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Text Box Border"];

        //This will most likely be changed to be more modular in the future, but for now it gets the job done.
        private int firstClick = 0;

        public TextBox(int x, int y, string defaultText, int maxLength, string textFilter = "", int width = 500, int height = 50) : base()
        {
            Font = width < 500 ? Globals.Hoog12 : Globals.Hoog24;
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            DefaultText = defaultText;
            Text = defaultText;
            MaxLength = maxLength;
            if (textFilter != "")
                TextFilter = textFilter;
        }
        public bool Focused { get; set; }
        public string DefaultText { get; set; }
        public int MaxLength { get; set; }
        public string TextFilter { get; set; } = @"^[a-zA-Z0-9.\-]*?$";

        public override void Update(GameTime gameTime)
        {
            ButtonBackCol = ColorManager.Instance.GuiColor["Text Box Background"];
            ButtonBorderCol = ColorManager.Instance.GuiColor["Text Box Border"];
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed &&
                OldMouseState.LeftButton == ButtonState.Released && !Hover)
            {
                Focused = false;
                Gui.Instance.CurrentTextBox = null;
                if (Text == "")
                {
                    Text = DefaultText;
                    firstClick = 0;
                }
            }
            base.Update(gameTime);
        }

        public override void ControlHover(object sender, Vector2 mousePos)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed &&
                OldMouseState.LeftButton == ButtonState.Released)
            {
                if (Text == DefaultText && firstClick == 0)
                {
                    Text = "";
                    firstClick++;
                }

                Gui.Instance.CurrentTextBox = this;
                Focused = true;
            }
            base.ControlHover(sender, mousePos);
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

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
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
            
            spriteBatch.DrawBorderedRect(new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), ButtonBackCol * mult, ButtonBorderCol * opacity);
            spriteBatch.DrawCenteredString(Font, cursor,
                new Vector2(Font.MeasureString(Text + cursor).X / 2 + (Position.X + Size.X/2), (Position.Y + Size.Y/2)),
                Color.LightGray);
            spriteBatch.DrawCenteredString(Font, Text, new Vector2(Position.X + Size.X/2, (Position.Y + Size.Y/2)),
                Color.LightGray);
            base.Draw(spriteBatch, gameTime);
        }
    }
}