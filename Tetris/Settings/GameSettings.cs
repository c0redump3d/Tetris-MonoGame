using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Tetris.GUI;
using Tetris.GUI.UiColor;

namespace Tetris.Settings;

/// <summary>
///     This class allows for custom settings to be saved to the settings xml file for the game.
/// </summary>
public class GameSettings
{
    private static GameSettings _instance;
    private readonly string appData; // appdata location
    public List<GameOption<Color>> ColorSettings = new();
    public List<GameOption<Keys>> KeybindSettings = new();
    public List<GameOption<float>> SliderSettings = new();
    public List<GameOption<bool>> ToggleSettings = new();

    public GameSettings()
    {
        appData = GetLocalAppDataFolder();
        RegisterSettings();
        Load();
    }

    public static GameSettings Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new GameSettings();

            return result;
        }
    }

    private string GetSettingsFile()
    {
        return Path.Combine(appData, "Tetris", "settings.xml");
    }

    private void RegisterSettings()
    {
        KeybindSettings = new List<GameOption<Keys>>();
        ToggleSettings = new List<GameOption<bool>>();
        SliderSettings = new List<GameOption<float>>();
        ColorSettings = new List<GameOption<Color>>();
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
        foreach (var control in ColorManager.Instance.GuiColor)
            ColorSettings.Add(new GameOption<Color>(control.Key, control.Value));
    }

    /// <summary>
    ///     Gives the current OS's application data folder.
    /// </summary>
    private string GetLocalAppDataFolder()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Environment.GetEnvironmentVariable("LOCALAPPDATA");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ??
                   Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".local", "share");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");

        //Game will unfortunately crash if the OS is not detected, but if able to debug will give exception error.
        throw new NotImplementedException("Unknown OS Platform");
    }

    private bool DoesFileExist()
    {
        if (File.Exists(GetSettingsFile()))
            return true;

        return false;
    }

    /// <summary>
    ///     Loads the settings file by deserializing the XML settings file.
    /// </summary>
    /// <exception cref="Exception">Will be raised if new settings are added and/or have been manipulated in some way.</exception>
    private void Load()
    {
        if (!DoesFileExist())
            Save();
        try
        {
            //Kind of gross but first time working with XML.
            using (var sr = new XmlTextReader(new FileStream(GetSettingsFile(), FileMode.Open)))
            {
                List<GameOption<Keys>> check1;
                List<GameOption<bool>> check2;
                List<GameOption<float>> check3;
                List<GameOption<Color>> check4;
                var extraTypes = new Type[4]
                {
                    typeof(List<GameOption<Keys>>), typeof(List<GameOption<bool>>), typeof(List<GameOption<float>>),
                    typeof(List<GameOption<Color>>)
                };
                var serializer = new XmlSerializer(typeof(List<object>), null, extraTypes,
                    new XmlRootAttribute("Options"), string.Empty);
                var list = new List<object>();
                list = (List<object>) serializer.Deserialize(sr);
                check1 = (List<GameOption<Keys>>) list[0];
                check2 = (List<GameOption<bool>>) list[1];
                check3 = (List<GameOption<float>>) list[2];
                check4 = (List<GameOption<Color>>) list[3];

                if (check1.Count != KeybindSettings.Count || check2.Count != ToggleSettings.Count ||
                    check3.Count != SliderSettings.Count ||
                    check4.Count != ColorSettings.Count) // if file is corrupted or does not match updated settings file
                    throw new Exception("Deserialized list does not match count of local settings!");

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
        if (!Directory.Exists(Path.Combine(appData, "Tetris")))
            Directory.CreateDirectory(Path.Combine(appData, "Tetris"));
        // Overrides the file if it already exists
        using (var sw = new StreamWriter(new FileStream(GetSettingsFile(), FileMode.Create)))
        {
            var extraTypes = new Type[4]
            {
                typeof(List<GameOption<Keys>>), typeof(List<GameOption<bool>>), typeof(List<GameOption<float>>),
                typeof(List<GameOption<Color>>)
            };
            var serializer = new XmlSerializer(typeof(List<object>), null, extraTypes, new XmlRootAttribute("Options"),
                string.Empty);
            var list = new List<object>();
            list.Add(KeybindSettings);
            list.Add(ToggleSettings);
            list.Add(SliderSettings);
            list.Add(ColorSettings);
            serializer.Serialize(sw, list);
        }

        Gui.Instance.AddDebugMessage($"Successfully saved settings file: {GetSettingsFile()}");
    }

    public void ChangeKeybind(string name, Keys value)
    {
        //Loop through our key options and set to new value
        foreach (var option in KeybindSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                option.SetValue(value);
    }

    public void ChangeToggle(string name, bool value)
    {
        foreach (var option in ToggleSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                option.SetValue(value);
    }

    public void ChangeSlider(string name, float value)
    {
        foreach (var option in SliderSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                option.SetValue(value);
    }

    public void ChangeColor(string name, Color col)
    {
        foreach (var option in ColorSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                option.SetValue(col);
    }

    /// <summary>
    ///     Returns the current value of the game setting (EX: "RotateLeft" could return Key "X")
    /// </summary>
    /// <param name="name">Setting Name</param>
    /// <returns>Object value</returns>
    public object GetOptionValue(string name)
    {
        foreach (var option in KeybindSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return option.GetValue();
        foreach (var option in ToggleSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return option.GetValue();
        foreach (var option in SliderSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return option.GetValue();
        foreach (var option in ColorSettings)
            if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return option.GetValue();

        return false;
    }
}