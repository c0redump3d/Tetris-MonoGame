using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.UiColor;
using Tetris.Sound;
using Tetris.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace Tetris.GUI.Control.Controls
{
    public class Button : UiControl
    {

        public Button(Vector2 pos, string text, SpriteFont font, bool showBox = true) : base()
        {
            Position = pos;
            Text = text;
            Font = font;
            ShowBox = showBox;
        }
        
        private int Growth { get; set; } = 1;
        private bool ShowBox { get; set; }
        private Vector2 box;
        public Action<object> OnClick;
        private Color ButtonBackCol = ColorManager.Instance.GuiColor["Button Background"];
        private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Button Border"];

        public void Draw(SpriteBatch spriteBatch, Color col)
        {
            var opacity = Gui.Instance.CurrentScreen.Opacity;

            var mult = opacity > 0.8f ? 0.8f : opacity - 0.2f;

            if (ShowBox)
                spriteBatch.DrawBorderedRect(new Rectangle((int)box.X, (int)box.Y,(int)Size.X,(int)Size.Y), ButtonBackCol * mult, ButtonBorderCol * opacity);

            spriteBatch.DrawCenteredString(Font, Text,
                new Vector2(Position.X, Position.Y + Font.MeasureString(Text).Y / 2),
                Enabled ? Hover ? Color.SlateGray * opacity : col * opacity : Color.Gray);
            
            base.Draw(spriteBatch);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            var opacity = Gui.Instance.CurrentScreen.Opacity;

            var mult = opacity > 0.8f ? 0.8f : opacity - 0.2f;

            if (ShowBox)
                spriteBatch.DrawBorderedRect(new Rectangle((int)box.X, (int)box.Y,(int)Size.X,(int)Size.Y), ButtonBackCol * mult, ButtonBorderCol * opacity);

            spriteBatch.DrawCenteredString(Font, Text,
                new Vector2(Position.X, Position.Y + Font.MeasureString(Text).Y / 2),
                Enabled ? Hover ? Color.SlateGray * opacity : Color.White * opacity : Color.Gray);
            
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            ButtonBackCol = ColorManager.Instance.GuiColor["Button Background"];
            ButtonBorderCol = ColorManager.Instance.GuiColor["Button Border"];
            
            if (Growth != 0 && !Hover)
            {
                Growth -= 1;
            }
            
            Vector2 boxSize = new()
            {
                X = (int) Font.MeasureString(Text).X + 6 + Growth * 2,
                Y = (int) Font.MeasureString(Text).Y + Growth * 2
            };
            Vector2 boxPos = new()
            {
                X = (Position.X * 2 - 4 - Growth - (int) Font.MeasureString(Text).X) / 2,
                Y = Position.Y - Growth
            };
            box = boxPos;
            Size = boxSize;
            // called every tick
            base.Update(gameTime);
        }

        public override RectangleF GetHoverRect()
        {
            return new (Position.X - Size.X/2, Position.Y, Size.X, Size.Y);
        }
        
        public override void ControlBeginHovering(object sender, Vector2 mousePos)
        {
            //Play hover sound
            Sfx.Instance.PlaySoundEffect("cursorhover");
            base.ControlBeginHovering(sender, mousePos);
        }

        public override void ControlHover(object sender, Vector2 mousePos)
        {
            if (Growth != 3) // used to scale the button's box
                Growth += 1;
            base.ControlHover(sender, mousePos);
        }

        public override void ControlClick(object sender, Vector2 mousePos)
        {
            base.ControlClick(sender, mousePos);
            OnClick?.Invoke(sender);
        }
    }
}