using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tetris.Game;
using Tetris.Settings;
using Tetris.Util;

namespace Tetris.GUI.UiColor
{
    /// <summary>
    /// This sets up and allows the user to control all specific color values of the GUI, pretty awesome and allows for lots of customization.
    /// </summary>
    public class ColorManager
    {
        /// <summary>
        /// Takes a string as the key which refers to the GUI element being edited, and the value is the Color value of the GUI element.
        /// </summary>
        public Dictionary<string, Color> GuiColor;

        private ColorManager()
        {
            GuiColor = new();
            GuiColor.Add("Button Background", new(63, 64, 65));
            GuiColor.Add("Button Border", new(97, 21, 179));
            GuiColor.Add("Slider Background", new(63, 64, 65));
            GuiColor.Add("Slider Border", new(97, 21, 179));
            GuiColor.Add("Text Box Background", new(63, 64, 65));
            GuiColor.Add("Text Box Border", new(97, 21, 179));
            GuiColor.Add("Panel Background", new(63, 64, 65));
            GuiColor.Add("Panel Border", new(97, 21, 179));
            GuiColor.Add("Game Box Background", new(63, 64, 65));
            GuiColor.Add("Game Box Border", new(97, 21, 179));
            GuiColor.Add("Game Box Grid", new(63, 64, 65));
            GuiColor.Add("Gradient Color 1", Color.Purple);
            GuiColor.Add("Gradient Color 2", Color.Black);
            GuiColor.Add("Score Background", new(63, 64, 65));
            GuiColor.Add("Score Border", new(97, 21, 179));
            GuiColor.Add("Score Text", new(97, 21, 179));
        }

        /// <summary>
        /// Reads and sets the UI Colors to whatever is currently saved in the game settings file.
        /// </summary>
        public void LoadColors()
        {
            foreach (KeyValuePair<string, Color> control in GuiColor)
            {
                if ((Color)GameSettings.Instance.GetOptionValue(control.Key) != control.Value)
                {
                    GuiColor[control.Key] = (Color) GameSettings.Instance.GetOptionValue(control.Key);
                }
            }
            Globals.GradientBackground.SetData(Utils.CreateGradient(GuiColor["Gradient Color 1"], GuiColor["Gradient Color 2"], 1280, 720));
        }
        
        private static ColorManager _instance;
        public static ColorManager Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new ColorManager();
                }

                return result;
            }
        }
    }
}