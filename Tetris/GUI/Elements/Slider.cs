using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.UiColor;
using Tetris.Util;

namespace Tetris.GUI.Elements
{
    public class Slider
    {
        private MouseState oldState;
        private float sliderValue;
        private bool dragging;
        private Rectangle SliderRec;
        public readonly string Name;
        /// <summary>
        /// A simple method to store extra information within a slider.
        /// </summary>
        public string Descriptor { get; set;  }

        public delegate void ReleaseMouse();
        public event ReleaseMouse OnRelease;
        public delegate void DraggingSlider(object sender);
        public event DraggingSlider Dragging;
        private Color ButtonBackCol = ColorManager.Instance.GuiColor["Slider Background"];
        private Color ButtonBorderCol = ColorManager.Instance.GuiColor["Slider Border"];
        
        public float X, Y;
        public int Width, Height;
        public float Multiplier = 100f;
        
        public Slider(float value, string name, float x, float y, int width = 200, int height = 30)
        {
            sliderValue = value;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            dragging = false;
            sliderValue = value;
            Name = name;
        }

        public float GetValue()
        {
            return sliderValue;
        }
        
        private void MouseInfo()
        {
            var mouseState = Mouse.GetState();
            var mousePos = Gui.Instance.TranslateMousePosition(mouseState);
            if (SliderRec.Contains(new Point((int) mousePos.X, (int) mousePos.Y)) &&
                Gui.Instance.CurrentScreen.Opacity >= 1f)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton != ButtonState.Pressed)
                {
                    dragging = true;
                    Dragging?.Invoke(this);
                    sliderValue = (mousePos.X - (X + 4)) / (Width - 10);
                    if (sliderValue < 0.0F)
                        sliderValue = 0.0F;

                    if (sliderValue > 1.0F)
                        sliderValue = 1.0F;
                }

                if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
                {
                    dragging = true;
                    Dragging?.Invoke(this);
                    sliderValue = (mousePos.X - (X + 4)) / (Width - 10);
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
            }
            else
            {
                if (dragging)
                {
                    dragging = false;
                    OnRelease?.Invoke();
                }
            }

            oldState = mouseState;
        }

        public void Update(GameTime gameTime)
        {
            ButtonBackCol = ColorManager.Instance.GuiColor["Slider Background"];
            ButtonBorderCol = ColorManager.Instance.GuiColor["Slider Border"];
            SliderRec = new Rectangle((int) X, (int) Y, Width, Height);
            MouseInfo();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var opacity = Gui.Instance.CurrentScreen.Opacity;
            var butMult = opacity > 0.8f ? 0.8f : opacity - 0.2f;
            var mult = opacity > 0.5f ? 0.8f : opacity - 0.2f;
            var sliderWidth = (Width * 0.05f);
            spriteBatch.DrawBorderedRect(SliderRec, ButtonBackCol*butMult, ButtonBorderCol*opacity);
            spriteBatch.DrawCenteredString(Globals.Hoog12, $"{Name}:{(int)(sliderValue*Multiplier)}", new Vector2(SliderRec.Center.X, SliderRec.Center.Y), Color.White * opacity);
            spriteBatch.DrawBorderedRect(new Rectangle((int) X + (int) (sliderValue * (Width - sliderWidth)), (int) Y,
                (int)(Width * 0.05f), Height), ButtonBackCol*mult, ButtonBorderCol*butMult);
        }
    }
}