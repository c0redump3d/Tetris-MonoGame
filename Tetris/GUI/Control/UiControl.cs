using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.Sound;

namespace Tetris.GUI.Control
{
    public class UiControl
    {
        //TODO: Needs basic control functions like hover, update, draw, click, w/h, x/y, font.
        /// <summary>
        /// Returns the assigned ID of the control
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Current X/Y position of the control
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Current Width/Height of the control
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// Returns whether or not the control can be interacted with.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Returns the current SpriteFont for text rendering.
        /// </summary>
        public SpriteFont Font { get; set; } = Globals.Hoog18;
        /// <summary>
        /// An assignable text variable used for drawing button text/category information.
        /// </summary>
        public string Text { get; set; } = "";
        /// <summary>
        /// Event is raised when the mouse begins hovering over control.
        /// </summary>
        private EventHandler<Vector2> BeginHover;
        /// <summary>
        /// Event is raised when the mouse is hovering over control.
        /// </summary>
        private EventHandler<Vector2> Hovering;
        /// <summary>
        /// Event is raised when the mouse is no longer hovering over control.
        /// </summary>
        private EventHandler<Vector2> HoverRelease;
        /// <summary>
        /// Event is raised the control is clicked.
        /// </summary>
        private EventHandler<Vector2> OnClick;
        /// <summary>
        /// Returns whether or not the control is being hovered.
        /// </summary>
        public bool Hover { get; set; }
        protected MouseState OldMouseState;

        public UiControl()
        {
            ID = Gui.Instance.CurrentScreen.TotalControls();
            Position = new();
            Size = new();
            BeginHover += ControlBeginHovering;
            HoverRelease += ControlStopHover;
            Hovering += ControlHover;
            OnClick += ControlClick;
        }

        private void HandleHover(MouseState mse)
        {
            var mousePos = Gui.Instance.TranslateMousePosition(mse);
            RectangleF controlBox = GetHoverRect();
            if (controlBox.Contains(new Point2(mousePos.X, mousePos.Y)) && Enabled && Gui.Instance.CurrentScreen.Opacity >= 1f)
            {
                if (!Hover)
                    BeginHover?.Invoke(this, mousePos);
                
                Hovering.Invoke(this, mousePos);

                if (OnClick != null && mse.LeftButton == ButtonState.Released &&
                    OldMouseState.LeftButton == ButtonState.Pressed)
                {
                    // Someone's listening, and we have a click
                    OnClick.Invoke(this, mousePos);
                }
            }
            else
            {
                HoverRelease?.Invoke(this, mousePos);
            }

        }

        public virtual RectangleF GetHoverRect()
        {
            return new (Position.X, Position.Y, Size.X, Size.Y);
        }

        public virtual void ControlBeginHovering(object sender, Vector2 mousePos)
        {
            Hover = true;
        }
        
        public virtual void ControlStopHover(object sender, Vector2 mousePos)
        {
            Hover = false;
        }
        
        public virtual void ControlHover(object sender, Vector2 mousePos)
        {
            
        }
        
        public virtual void ControlClick(object sender, Vector2 mousePos)
        {
            if (Gui.Instance.CurrentScreen.Closing)
                return;
            //Play click sound event
            Sfx.Instance.PlaySoundEffect("click");
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (DebugMenu.DebugMenu.Instance.IsOptionEnabled(4))
            {
                var mouseState = Mouse.GetState();
                var mousePos = Gui.Instance.TranslateMousePosition(mouseState);
                if (Hover)
                {
                    var text =
                        $"Button ID:{ID} X:{Position.X} Y:{Position.Y}\nEnabled:{Enabled} WxH:{Size.X}x{Size.Y}";
                    Rectangle debugRect =
                        new((int) mousePos.X - 32, (int) mousePos.Y - 32, (int) Globals.ConsoleFont.MeasureString(text)
                            .X + 4, (int) Globals.ConsoleFont.MeasureString(text).Y);
                    spriteBatch.Draw(Globals.TexBox, debugRect, Color.Black * 0.5f);
                    spriteBatch.DrawString(Globals.ConsoleFont, $"{text}",
                        new Vector2(mousePos.X - 30, mousePos.Y - 30), Color.White);
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Draw(spriteBatch);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Gui.Instance.CurrentScreen.Closing)
                return;
            MouseState mse = Mouse.GetState();
            //Handle hovering of control.
            HandleHover(mse);
            OldMouseState = mse;
        }
    }
}