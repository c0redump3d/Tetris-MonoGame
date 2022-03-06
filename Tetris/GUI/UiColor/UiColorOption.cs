using System;
using System.Collections.Generic;
using Autofac.Core.Activators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.GUI.Control.Controls;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.GUI.UiColor
{
    /// <summary>
    /// Creates a specific panel used for managing a specific GUI elements color.
    /// Has features like minimizing the window, dragging/moving window, and category support!
    /// This was extremely tedious to set up D:
    /// </summary>
    public class UiColorOption
    {
        //TODO: Add ActiveControl function, so panels below other panels are not changed.
        
        public string CategoryName { get; set; }
        public Vector2 Position { get; set; }
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }
        public int SliderWidth { get; set; }
        public int Spacing = 200;

        private Button minimizeBut;
        private bool hidden = true;
        private MouseState oldMouseState;
        private Vector2 origClick;
        public bool Dragging { get; set; }
        private bool CanDrag() => Gui.Instance.CurrentColorPanel is { Dragging: false };
        public bool IsActive() => (Gui.Instance.CurrentColorPanel != null) && (Gui.Instance.CurrentColorPanel == this);
        
        //This is the main panel that holds all the sliders for the color.
        public Dictionary<string, object[]> CategoryControl;
        public string ParentName = "";
        public List<Slider> OptionSliders;

        public UiColorOption(string category, Vector2 pos, int sliderWidth)
        {
            Gui.Instance.CurrentColorPanel = null;
            OptionSliders = new List<Slider>();
            CategoryControl = new();
            CategoryName = category;
            Position = pos;
            SliderWidth = sliderWidth;
            foreach (KeyValuePair<string, Color> control in ColorManager.Instance.GuiColor)
            {
                if (control.Key.Split(' ')[0].Equals(category))
                {
                    object[] catInfo = IsParent(control.Key);
                    int categoryAmount = (int) catInfo[1];
                    bool parent = (bool) catInfo[0];
                    if (parent)
                    {
                        ParentName = control.Key;
                    }
                    CategoryControl.Add(control.Key, new []{(Object)control.Value, categoryAmount});
                    PanelWidth = (SliderWidth * (categoryAmount)) + 40 + categoryAmount > 1
                        ? (categoryAmount * Spacing)
                        : 0;
                    PanelHeight = 25;
                }
            }

            minimizeBut = new Button(new Vector2(Position.X + (PanelWidth - 12), Position.Y + 3), "+",
                Globals.Hoog12, false);
            minimizeBut.OnClick += o =>
            {
                var panelHeight = PanelHeight == 145 ? PanelHeight = 25 : PanelHeight = 145;
                PanelHeight = panelHeight;
                hidden = !hidden;
                minimizeBut.Text = hidden ? "+" : "-";
            };
            AddCategory();
        }

        private void AddCategory()
        {
            int count = 0;
            int x = (int)Position.X + 20;
            int y = (int)Position.Y + 25;
            foreach (KeyValuePair<string, object[]> control in CategoryControl)
            {
                AddColorOption(control.Key, ((Color)control.Value[0]).ToVector3(), x,y);
                x += Spacing;
                count++;
            }
        }
        
        private void AddColorOption(string name, Vector3 globalColor, int x, int y)
        {
            string[] colors = {"Red","Green","Blue"};
            float[] rgb = {globalColor.X, globalColor.Y, globalColor.Z};
            for (int i = 0; i < 3; i++)
            {
                Slider slider;
                OptionSliders.Add(slider = new Slider(rgb[i], colors[i], x, y + (i*40), SliderWidth) { Descriptor = name, Multiplier = 255f });
                slider.Dragging += UpdateColors;
                slider.OnRelease += () => GameSettings.Instance.Save();
            }
        }

        private void UpdatePositions()
        {
            int count = 0;
            int x = (int)Position.X + 20;
            int y = (int)Position.Y + 25;
            foreach (Slider slide in OptionSliders)
            {
                slide.Position = new Vector2(x, y + (count * 40));
                count++;
                if (count == 3)
                {
                    count = 0;
                    x += Spacing;
                }
            }

            minimizeBut.Position = new Vector2(Position.X + (PanelWidth - 12), Position.Y + 3);
        }
        
        private void UpdateColors(object sender)
        {
            Slider slider = (Slider) sender;
            //Store the color's RGB values into a vector.
            Vector3 colorToChange = ((Color)CategoryControl[slider.Descriptor][0]).ToVector3();
            if (slider.Text.Contains("Red"))
            {
                colorToChange.X = slider.GetValue();
            }else if (slider.Text.Contains("Green"))
            {
                colorToChange.Y = slider.GetValue();
            }else if (slider.Text.Contains("Blue"))
            {
                colorToChange.Z = slider.GetValue();
            }

            var col = new Color(colorToChange);
            CategoryControl[slider.Descriptor][0] = col;
            ColorManager.Instance.GuiColor[slider.Descriptor] = col;
            //This is not a great way of changing the gradient, uses lots of memory.
            //TODO: Can't change this right now, but make a check to only update this when gradient is being changed, not any color.
            Globals.GradientBackground.SetData(Utils.CreateGradient(ColorManager.Instance.GuiColor["Gradient Color 1"], ColorManager.Instance.GuiColor["Gradient Color 2"], 1280, 720));
            GameSettings.Instance.ChangeColor(slider.Descriptor, col);
        }


        public void Update(GameTime gameTime)
        {
            MouseState mse = Mouse.GetState();

            if(IsActive())
                minimizeBut.Update(gameTime);

            //Enable dragging
            SetActive(mse);
            HandleDrag(mse);

            if (!hidden)
            {
                foreach (Slider slider in OptionSliders)
                {
                    if (!IsActive() && slider.Enabled)
                        slider.Enabled = false;
                    else if (CanDrag() && !slider.Enabled)
                        slider.Enabled = true;
                    if(IsActive())
                        slider.Update(gameTime);
                }
            }
            
            oldMouseState = mse;
        }

        private void SetActive(MouseState mse)
        {
            if (Gui.Instance.CurrentColorPanel == null)
            {
                Gui.Instance.CurrentColorPanel = this;
                return;
            }

            var curPanel = Gui.Instance.CurrentColorPanel;
            Vector2 mouseVec = Gui.Instance.TranslateMousePosition(mse);
            RectangleF panelPos = new RectangleF(Position.X, Position.Y, PanelWidth, PanelHeight);
            RectangleF mousePos = new RectangleF(mouseVec.X, mouseVec.Y, 1, 1);
            RectangleF activePanel = new RectangleF(curPanel.Position.X, curPanel.Position.Y, curPanel.PanelWidth,
                curPanel.PanelHeight);

            //Check to see if active panel is above any other panels, if so we don't want the panel below becoming parent.
            if (activePanel.Intersects(mousePos))
                return;

            if (panelPos.Intersects(mousePos))
            {
                if (oldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                {
                    if (!Dragging)
                    {
                        Gui.Instance.CurrentColorPanel = this;
                    }
                }
            }
        }
        
        private void HandleDrag(MouseState mse)
        {
            if (Gui.Instance.CurrentColorPanel == this)
            {
                Vector2 mousePos = Gui.Instance.TranslateMousePosition(mse);
                RectangleF panelPos = new RectangleF(Position.X, Position.Y, PanelWidth, 25);

                if (panelPos.Intersects(new RectangleF(mousePos.X, mousePos.Y, 1, 1)))
                {
                    if (oldMouseState.LeftButton == ButtonState.Pressed && mse.LeftButton == ButtonState.Pressed)
                    {
                        if (!Dragging)
                        {
                            origClick = new Vector2((mousePos.X - panelPos.X), mousePos.Y - panelPos.Y);
                        }

                        Dragging = true;
                    }
                    else
                    {
                        Dragging = false;
                        MoveOnScreen();
                    }
                }

                if (Dragging)
                {
                    panelPos.X = mousePos.X - origClick.X;
                    panelPos.Y = mousePos.Y - origClick.Y;
                    Position = new Vector2(panelPos.X, panelPos.Y);
                    UpdatePositions();
                }
            }
        }

        private void MoveOnScreen()
        {
            var offScreen = Position;
            if (Position.X > 1280 - (PanelWidth / 2f))
                offScreen.X = 1280 - PanelWidth / 2f;
            
            if (Position.X + PanelWidth/2f < 0)
                offScreen.X = -(PanelWidth/2f);

            if (Position.Y > 720 - (PanelHeight / 2f))
                offScreen.Y = 720 - PanelHeight / 2f;
            
            if (Position.Y < 5)
                offScreen.Y = 5;
            Position = offScreen;
            UpdatePositions();
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            var ButtonBackCol = ColorManager.Instance.GuiColor["Panel Background"];
            var ButtonBorderCol = ColorManager.Instance.GuiColor["Panel Border"];
            minimizeBut.Draw(spriteBatch, Color.White);
            
            foreach (KeyValuePair<string, object[]> control in CategoryControl)
            {
                var opac = Gui.Instance.CurrentScreen.Opacity;
                float backOpac = IsActive() ? 0.5f : 0.3f;
                var mult = opac > backOpac ? backOpac : opac;
                if (ParentName.Equals(control.Key))
                {
                    int recX = (int)Position.X;
                    int recY = (int)Position.Y;
                    spriteBatch.DrawBorderedRect(
                        new Rectangle(recX, recY,
                            PanelWidth, PanelHeight),
                        ButtonBackCol * mult,
                        ButtonBorderCol * opac);
                }

                if (!hidden)
                {
                    Vector2 pos = GetNamePos(control.Key);
                    spriteBatch.DrawCenteredString(Globals.Hoog12, control.Key,
                        new Vector2(pos.X, pos.Y), (Color)control.Value[0]);
                }
            }

            if (hidden)
            {
                spriteBatch.DrawCenteredString(Globals.Hoog12, ParentName, new Vector2(Position.X + (PanelWidth/2f), Position.Y + 12), Color.White);
            }

            if (!hidden)
            {
                foreach (Slider slider in OptionSliders)
                {
                    slider.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Returns a vector that is in the top-center of the created panel.
        /// </summary>
        private Vector2 GetNamePos(string name)
        {
            foreach (Slider slider in OptionSliders)
            {
                if (slider.Descriptor.Equals(name) && slider.Text.Contains("Red"))
                {
                    return new Vector2(slider.Position.X + (SliderWidth / 2f), slider.Position.Y - 15);
                }
            }

            return new Vector2(0, 0);
        }
        
        private object[] IsParent(string name)
        {
            string category = name.Split(' ')[0];
            int categorysize = 0;
            string parent = String.Empty;
            foreach (KeyValuePair<string, Color> tempControl in ColorManager.Instance.GuiColor)
            {
                if (tempControl.Key.Split(' ')[0].Contains(category))
                {
                    if (parent.Equals(String.Empty))
                    {
                        parent = tempControl.Key;
                    }
                    categorysize++;
                }
            }

            return new []{(Object)(name.Equals(parent)), categorysize};
        }
    }
}