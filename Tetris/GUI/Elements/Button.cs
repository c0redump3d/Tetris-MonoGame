using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.UiColor;
using Tetris.Sound;
using Tetris.Util;
using static Microsoft.Xna.Framework.Input.ButtonState;

namespace Tetris.GUI.Elements
{
    public class Button
    {
        private MouseState oldState;

        public Button(int id, Vector2 pos, string text, SpriteFont font, bool showBox = true)
        {
            Id = id;
            Pos = pos;
            Text = text;
            Font = font;
            ShowBox = showBox;
        }

        public Vector2 Pos { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Box { get; set; }
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public bool Hovering { get; private set; }
        public bool Enabled { get; set; } = true;
        public int Size { get; set; } = 1;
        public int Id { get; set; }
        public bool ShowBox { get; set; } = true;
        public event Action<object> OnClick;
        private Color ButtonBackCol = ColorManager.Instance.GuiColor["Button Background"];
        private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Button Border"];

        public void Draw(SpriteBatch spriteBatch, Color col)
        {
            var opacity = Gui.Instance.CurrentScreen.Opacity;

            var mult = opacity > 0.8f ? 0.8f : opacity - 0.2f;

            if (ShowBox)
                spriteBatch.DrawBorderedRect(Box, ButtonBackCol * mult, ButtonBorderCol * opacity);

            spriteBatch.DrawCenteredString(Font, Text,
                new Vector2(Pos.X, Pos.Y + Font.MeasureString(Text).Y / 2),
                Enabled ? Hovering ? Color.SlateGray * opacity : col * opacity : Color.Gray);

            if (DebugMenu.DebugMenu.Instance.IsOptionEnabled(4))
            {
                var mouseState = Mouse.GetState();
                var mousePos = Gui.Instance.TranslateMousePosition(mouseState);
                if (Box.Contains(mousePos))
                {
                    var text =
                        $"Button ID:{Id} X:{Pos.X} Y:{Pos.Y}\nEnabled:{Enabled} WxH:{Width}x{Height}";
                    Rectangle debugRect =
                        new((int) mousePos.X - 32, (int) mousePos.Y - 32, (int) Globals.ConsoleFont.MeasureString(text)
                            .X + 4, (int) Globals.ConsoleFont.MeasureString(text).Y);
                    spriteBatch.Draw(Globals.TexBox, debugRect, Color.Black * 0.5f);
                    spriteBatch.DrawString(Globals.ConsoleFont, $"{text}",
                        new Vector2(mousePos.X - 30, mousePos.Y - 30), Color.White);
                }
            }
        }

        public void Update()
        {
            // called every tick
            if (Gui.Instance.CurrentScreen.Closing)
                return;
            ButtonBackCol = ColorManager.Instance.GuiColor["Button Background"];
            ButtonBorderCol = ColorManager.Instance.GuiColor["Button Border"];

            var mouseState = Mouse.GetState();
            var mousePos = Gui.Instance.TranslateMousePosition(mouseState);
            Width = (int) Font.MeasureString(Text).X;
            Height = (int) Font.MeasureString(Text).Y;
            Box = new Rectangle((int) (Pos.X * 2 - 4 - Size - (int) Font.MeasureString(Text).X) / 2, (int) Pos.Y - Size,
                Width + 6 + Size * 2, Height + Size * 2);
            if (Box.Contains(new Point((int) mousePos.X, (int) mousePos.Y)) && Enabled && Gui.Instance.CurrentScreen.Opacity >= 1f)
            {
                if (!Hovering)
                    Sfx.Instance.PlaySoundEffect("cursorhover");

                Hovering = true;

                if (Size != 3) // used to scale the button's box
                    Size += 1;

                if (OnClick != null && mouseState.LeftButton == Released &&
                    oldState.LeftButton == Pressed)
                {
                    // Someone's listening, and we have a click
                    Sfx.Instance.PlaySoundEffect("click");
                    OnClick.Invoke(this);
                }
            }
            else
            {
                if (Size != 0) Size -= 1;

                Hovering = false;
            }

            oldState = mouseState;
        }
    }
}