using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Elements;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.GUI.UiColor
{
    public class UiColorOption
    {
        public string CategoryName { get; set; }
        public Vector2 Position { get; set; }
        public int PanelWidth { get; set; }
        public int PanelHeight { get; set; }
        public int SliderWidth { get; set; }
        public int Spacing = 200;

        public Dictionary<string, object[]> CategoryControl;
        public string ParentName = "";
        public List<Slider> OptionSliders;

        public UiColorOption(string category, Vector2 pos, int sliderWidth)
        {
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
                }
            }
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
        
        private void UpdateColors(object sender)
        {
            Slider slider = (Slider) sender;
            Vector3 colorToChange = ((Color)CategoryControl[slider.Descriptor][0]).ToVector3();
            if (slider.Name.Contains("Red"))
            {
                colorToChange.X = slider.GetValue();
            }else if (slider.Name.Contains("Green"))
            {
                colorToChange.Y = slider.GetValue();
            }else if (slider.Name.Contains("Blue"))
            {
                colorToChange.Z = slider.GetValue();
            }

            var col = new Color(colorToChange);
            CategoryControl[slider.Descriptor][0] = col;
            ColorManager.Instance.GuiColor[slider.Descriptor] = col;
            Globals.GradientBackground.SetData(Utils.CreateGradient(ColorManager.Instance.GuiColor["Gradient Color 1"], ColorManager.Instance.GuiColor["Gradient Color 2"], 1280, 720));
            GameSettings.Instance.ChangeColor(slider.Descriptor, col);
        }


        public void Update(GameTime gameTime)
        {
            foreach (Slider slider in OptionSliders)
            {
                slider.Update(gameTime);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            var ButtonBackCol = ColorManager.Instance.GuiColor["Panel Background"];
            var ButtonBorderCol = ColorManager.Instance.GuiColor["Panel Border"];
            int count = 0;
            foreach (KeyValuePair<string, object[]> control in CategoryControl)
            {
                var opac = Gui.Instance.CurrentScreen.Opacity;
                var mult = opac > 0.5f ? 0.5f : opac;
                if (ParentName.Equals(control.Key))
                {
                    int recX = (int)Position.X;
                    int recY = (int)Position.Y;
                    spriteBatch.DrawBorderedRect(
                        new Rectangle(recX, recY,
                            PanelWidth, PanelHeight = 145),
                        ButtonBackCol * mult,
                        ButtonBorderCol * opac);
                }
                Vector2 pos = GetNamePos(control.Key);
                spriteBatch.DrawCenteredString(Globals.Hoog12, control.Key,
                    new Vector2(pos.X, pos.Y), (Color)control.Value[0]);
                count++;
            }
            
            foreach (Slider slider in OptionSliders)
            {
                slider.Draw(spriteBatch);
            }
        }

        private Vector2 GetNamePos(string name)
        {
            foreach (Slider slider in OptionSliders)
            {
                if (slider.Descriptor.Equals(name) && slider.Name.Contains("Red"))
                {
                    return new Vector2(slider.X + (SliderWidth / 2f), slider.Y - 15);
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