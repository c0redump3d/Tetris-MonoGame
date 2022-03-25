using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls;

public class TextBox : UiControl
{
    private Color ButtonBackCol = ColorManager.Instance.GuiColor["Text Box Background"];
    private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Text Box Border"];
    private double cursorTime;

    private bool firstClick;

    public TextBox(int x, int y, string defaultText, int maxLength, string textFilter = "", int width = 500,
        int height = 50)
    {
        Font = width < 500 ? Fonts.Hoog12 : Fonts.Hoog24;
        Position = new Vector2(x, y);
        Size = new Vector2(width, height);
        DefaultText = defaultText;
        Text = defaultText;
        MaxLength = maxLength;
        if (textFilter != "")
            TextFilter = textFilter;
    }

    public bool Focused { get; set; }
    private string DefaultText { get; }
    private int MaxLength { get; }
    private string TextFilter { get; } = @"^[a-zA-Z0-9.\-]*?$";

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
                firstClick = false;
            }
        }

        base.Update(gameTime);
    }

    protected override void ControlBeginHovering(object sender, Vector2 mousePos)
    {
        Mouse.SetCursor(MouseCursor.IBeam);
        base.ControlBeginHovering(sender, mousePos);
    }

    protected override void ControlStopHover(object sender, Vector2 mousePos)
    {
        Mouse.SetCursor(MouseCursor.Arrow);
        base.ControlStopHover(sender, mousePos);
    }

    protected override void ControlHover(object sender, Vector2 mousePos)
    {
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed &&
            OldMouseState.LeftButton == ButtonState.Released)
        {
            if (Text == DefaultText && !firstClick)
            {
                Text = "";
                firstClick = true;
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

        var stringIsValid = Regex.IsMatch(key.ToString(), @$"{TextFilter}");

        if (Text.Length < MaxLength && stringIsValid)
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

        spriteBatch.DrawBorderedRect(new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, (int) Size.Y),
            ButtonBackCol * mult, ButtonBorderCol * opacity);
        spriteBatch.DrawCenteredString(Font, cursor,
            new Vector2(Font.MeasureString(Text + cursor).X / 2 + (Position.X + Size.X / 2), Position.Y + Size.Y / 2),
            Color.LightGray);
        spriteBatch.DrawCenteredString(Font, Text, new Vector2(Position.X + Size.X / 2, Position.Y + Size.Y / 2),
            Color.LightGray);
        base.Draw(spriteBatch, gameTime);
    }
}