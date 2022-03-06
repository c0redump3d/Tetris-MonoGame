using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls
{
    public class Slider : UiControl
    {
        private MouseState oldState;
        private float sliderValue;
        private bool dragging;
        private Rectangle SliderRec;
        /// <summary>
        /// A simple method to store extra information within a slider.
        /// </summary>
        public string Descriptor { get; set;  }

        public delegate void ReleaseMouse();
        public event ReleaseMouse OnRelease;
        public event DraggingSlider Dragging;
        public delegate void DraggingSlider(object sender);
        private Color SliderBackCol = ColorManager.Instance.GuiColor["Slider Background"];
        private Color SliderBorderCol = ColorManager.Instance.GuiColor["Slider Border"];
        public float Multiplier = 100f;
        
        public Slider(float value, string name, float x, float y, int width = 200, int height = 30) : base()
        {
            sliderValue = value;
            Position = new Vector2(x, y);
            Size = new Vector2(width, height);
            dragging = false;
            sliderValue = value;
            Text = name;
        }

        public float GetValue()
        {
            return sliderValue;
        }

        public override void ControlHover(object sender, Vector2 mousePos)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed)
            {
                dragging = true;
                Dragging?.Invoke(this);
                sliderValue = (mousePos.X - (Position.X + 4)) / (Size.X - 10);
                if (sliderValue < 0.0F)
                    sliderValue = 0.0F;

                if (sliderValue > 1.0F)
                    sliderValue = 1.0F;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
            {
                dragging = true;
                Dragging?.Invoke(this);
                sliderValue = (mousePos.X - (Position.X + 4)) / (Size.X - 10);
                if (sliderValue < 0.0F)
                {
                    sliderValue = 0.0F;
                }

                if (sliderValue > 1.0F)
                {
                    sliderValue = 1.0F;
                }
            }

            if (mouseState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed)
            {
                dragging = false;
                OnRelease?.Invoke();
            }

            oldState = mouseState;
            base.ControlHover(sender, mousePos);
        }

        public override void ControlStopHover(object sender, Vector2 mousePos)
        {
            if (dragging)
            {
                dragging = false;
                OnRelease?.Invoke();
            }
            base.ControlStopHover(sender, mousePos);
        }

        public override void Update(GameTime gameTime)
        {
            SliderBackCol = ColorManager.Instance.GuiColor["Slider Background"];
            SliderBorderCol = ColorManager.Instance.GuiColor["Slider Border"];
            SliderRec = new Rectangle((int) Position.X, (int) Position.Y, (int)Size.X, (int)Size.Y);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var opacity = Gui.Instance.CurrentScreen.Opacity;
            var butMult = opacity > 0.8f ? 0.8f : opacity - 0.2f;
            var mult = opacity > 0.5f ? 0.8f : opacity - 0.2f;
            var sliderWidth = (Size.X * 0.05f);
            spriteBatch.DrawBorderedRect(SliderRec, SliderBackCol*butMult, SliderBorderCol*opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog12, $"{Text}:{(int)(sliderValue*Multiplier)}", new Vector2(SliderRec.Center.X, SliderRec.Center.Y), Color.White * opacity);
            spriteBatch.DrawBorderedRect(new Rectangle((int) Position.X + (int) (sliderValue * (Size.X - sliderWidth)), (int) Position.Y,
                (int)(Size.X * 0.05f), (int)Size.Y), SliderBackCol*mult, SliderBorderCol*butMult);
            base.Draw(spriteBatch);
        }
    }
}