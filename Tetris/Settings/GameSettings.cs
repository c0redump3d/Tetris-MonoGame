using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tetris.Game.Managers;
using Tetris.GUI.DebugMenu;
using Tetris.GUI.UiColor;

namespace Tetris.Settings
{
    public class GameSettings
    {
        public List<GameOption<Keys>> KeybindSettings = new();
        public List<GameOption<bool>> ToggleSettings = new();
        public List<GameOption<float>> SliderSettings = new();
        public List<GameOption<Color>> ColorSettings = new();
        private readonly string appData; // appdata location
        private string GetSettingsFile() => appData + @"\Tetris\settings.xml";
        
        public GameSettings()
        {
            appData = GetLocalAppDataFolder();
            RegisterSettings();
            Load();
        }

        private void RegisterSettings()
        {
            KeybindSettings = new();
            ToggleSettings = new();
            SliderSettings = new();
            ColorSettings = new();
            KeybindSettings.Add(new GameOption<Keys>("Left", Keys.A));
            KeybindSettings.Add(new GameOption<Keys>("Right", Keys.D));
            KeybindSettings.Add(new GameOption<Keys>("Down", Keys.S));
            KeybindSettings.Add(new GameOption<Keys>("RotateRight", Keys.X));
            KeybindSettings.Add(new GameOption<Keys>("RotateLeft", Keys.Z));
            KeybindSettings.Add(new GameOption<Keys>("HardDrop", Keys.Space));
            KeybindSettings.Add(new GameOption<Keys>("Hold", Keys.LeftShift));
            ToggleSettings.Add(new GameOption<bool>("Music", true));
            SliderSettings.Add(new GameOption<float>("Volume", 0.1f));
            ToggleSettings.Add(new GameOption<bool>("Fullscreen", false));
            foreach(KeyValuePair<string, Color> control in ColorManager.Instance.GuiColor)
                ColorSettings.Add(new GameOption<Color>(control.Key, control.Value));
        }
        
        private string GetLocalAppDataFolder()
        {
            #if __IOS__
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            #endif
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Environment.GetEnvironmentVariable("LOCALAPPDATA");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ??
                       Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".local", "share");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");
            
            throw new NotImplementedException("Unknown OS Platform");
        }
        
        private bool DoesFileExist()
        {
            if (File.Exists(appData + @"\Tetris\settings.xml"))
                return true;
            
            return false;
        }

        private void Load()
        {
            if (!DoesFileExist())
                Save();
            try
            {
                using (var sr = new XmlTextReader(new FileStream(GetSettingsFile(), FileMode.Open)))
                {
                    List<GameOption<Keys>> check1;
                    List<GameOption<bool>> check2;
                    List<GameOption<float>> check3;
                    List<GameOption<Color>> check4;
                    Type[] extraTypes = new Type[4]
                    {
                        typeof(List<GameOption<Keys>>), typeof(List<GameOption<bool>>), typeof(List<GameOption<float>>), typeof(List<GameOption<Color>>)
                    };
                    XmlSerializer serializer = new XmlSerializer(typeof(List<object>), null, extraTypes,
                        new XmlRootAttribute("Options"), string.Empty);
                    List<object> list = new List<object>();
                    list = (List<object>) serializer.Deserialize(sr);
                    check1 = (List<GameOption<Keys>>) list[0];
                    check2 = (List<GameOption<bool>>) list[1];
                    check3 = (List<GameOption<float>>) list[2];
                    check4 = (List<GameOption<Color>>) list[3];
                    
                    if (check1.Count != KeybindSettings.Count || check2.Count != ToggleSettings.Count ||
                        check3.Count != SliderSettings.Count || check4.Count != ColorSettings.Count) // if file is corrupted or does not match updated settings file
                    {
                        throw new Exception("Deserialized list does not match count of local settings!");
                    }

                    KeybindSettings = check1;
                    ToggleSettings = check2;
                    SliderSettings = check3;
                    ColorSettings = check4;
                }
            }
            catch (Exception)
            {
                //Reset the file if exception is raised..
                File.Delete(GetSettingsFile());
            }

            Save();
        }

        public void Reset()
        {
            RegisterSettings();
            File.Delete(GetSettingsFile());
            Load();
        }
        
        public void Save()
        {
            if (!Directory.Exists(appData + @"\Tetris"))
                Directory.CreateDirectory(appData + @"\Tetris");
            // Overrides the file if it alreadt exists
            using (var sw = new StreamWriter(new FileStream(GetSettingsFile(), FileMode.Create)))
            {
                Type[] extraTypes = new Type[4] { typeof(List<GameOption<Keys>>), typeof(List<GameOption<bool>>), typeof(List<GameOption<float>>), typeof(List<GameOption<Color>>) };
                XmlSerializer serializer = new XmlSerializer(typeof(List<object>), null, extraTypes, new XmlRootAttribute("Options"), string.Empty);
                List<object> list = new List<object>();
                list.Add(KeybindSettings);
                list.Add(ToggleSettings);
                list.Add(SliderSettings);
                list.Add(ColorSettings);
                serializer.Serialize(sw, list);
            }
            DebugConsole.Instance.AddMessage("Successfully saved settings file.");
        }
        
        public void ChangeKeybind(string name, Keys value)
        {
            foreach (GameOption<Keys> option in KeybindSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    option.SetValue(value);
                }
            }
        }
        
        public void ChangeToggle(string name, bool value)
        {
            foreach (GameOption<bool> option in ToggleSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    option.SetValue(value);
                }
            }
        }
        
        public void ChangeSlider(string name, float value)
        {
            foreach (GameOption<float> option in SliderSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    option.SetValue(value);
                }
            }
        }
        
        public void ChangeColor(string name, Color col)
        {
            foreach (GameOption<Color> option in ColorSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    option.SetValue(col);
                }
            }
        }

        public object GetOptionValue(string name)
        {
            foreach (GameOption<Keys> option in KeybindSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return option.GetValue();
                }
            }
            foreach (GameOption<bool> option in ToggleSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return option.GetValue();
                }
            }
            foreach (GameOption<float> option in SliderSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return option.GetValue();
                }
            }
            foreach (GameOption<Color> option in ColorSettings)
            {
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return option.GetValue();
                }
            }

            return false;
        }

        private static GameSettings _instance;
        public static GameSettings Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new GameSettings();
                }

                return result;
            }
        }
    }
}