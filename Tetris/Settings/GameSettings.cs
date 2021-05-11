using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace Tetris.Settings
{
    public class GameSettings
    {
        //TODO: Add hold button and counterclockwise rotate
        
        private readonly string appData; // appdata location

        public GameSettings()
        {
            appData = GetLocalAppDataFolder();
            try
            {
                if(!CreateSettingsFile())
                    LoadSettings();
            }catch(Exception)
            {
                File.Delete(appData + @"\TetrisGame\settings.tet");
                CreateSettingsFile();
            }

        }

        string GetLocalAppDataFolder() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.GetEnvironmentVariable("LOCALAPPDATA");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ?? Path.Combine(Environment.GetEnvironmentVariable("HOME"),".local","share");
            } 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");
            }
            throw new NotImplementedException("Unknown OS Platform");
        }
        
        private bool CreateSettingsFile()
        {
            if (File.Exists(appData + @"\TetrisGame\settings.tet"))
                return false;

            new MovementKeys(Keys.A, Keys.D, Keys.S, Keys.X, Keys.Z, Keys.Space, Keys.LeftShift);
            new AudioSettings(50, 1);

            if (!Directory.Exists(appData + @"\TetrisGame\"))
                Directory.CreateDirectory(appData + @"\TetrisGame\");

            var settingsFile = File.Create(appData + @"\TetrisGame\settings.tet");

            settingsFile.Close();

            SaveSettings();

            return true;
        }

        public void SaveSettings()
        {
            using(StreamWriter sw = new StreamWriter(appData + @"\TetrisGame\settings.tet"))
            {
                foreach(KeyValuePair<string, Keys> con in MovementKeys.CONTROLS)
                {
                    sw.WriteLine(con.Key + ":" + con.Value);
                }
                foreach (KeyValuePair<string, int> audio in AudioSettings.AUDIO)
                {
                    sw.WriteLine(audio.Key + ":" + audio.Value);
                }
            }
        }

        private void LoadSettings()
        {
            //Reads settings file and sets keys and audio
            int count = 0;
            Dictionary<string, Keys> keys = new Dictionary<string, Keys>();
            Dictionary<string, int> audio = new Dictionary<string, int>();
            using (StreamReader sr = new StreamReader(appData + @"\TetrisGame\settings.tet"))
            {
                string val;
                while((val = sr.ReadLine()) != null)
                {
                    string[] data = val.Split(':');
                    if (count < 7)
                    {
                        keys.Add(data[0], (Keys)Enum.Parse(typeof(Keys), data[1]));
                        count++;
                    }
                    else
                    {
                        audio.Add(data[0], int.Parse(data[1]));
                    }
                }
            }

            new MovementKeys(keys["Left"], keys["Right"], keys["Down"], keys["RotateRight"], keys["RotateLeft"], keys["Forcedrop"], keys["Hold"]);
            new AudioSettings(audio["Volume"], audio["Music"]);

        }

    }

    public struct MovementKeys
    {
        public static Keys LEFT;
        public static Keys RIGHT;
        public static Keys DOWN;
        public static Keys ROTATERIGHT;
        public static Keys ROTATELEFT;
        public static Keys FORCEDROP;
        public static Keys HOLD;
        public static readonly Dictionary<string, Keys> CONTROLS = new Dictionary<string, Keys>();

        public MovementKeys(Keys left, Keys right, Keys down, Keys rotateright, Keys rotateleft, Keys forcedrop, Keys hold)
        {
            LEFT = left;
            RIGHT = right;
            DOWN = down;
            ROTATERIGHT = rotateright;
            ROTATELEFT = rotateleft;
            FORCEDROP = forcedrop;
            HOLD = hold;
            createDic();
        }

        private void createDic()
        {
            CONTROLS.Clear();
            CONTROLS.Add("Left", LEFT);
            CONTROLS.Add("Right", RIGHT);
            CONTROLS.Add("Down", DOWN);
            CONTROLS.Add("RotateRight", ROTATERIGHT);
            CONTROLS.Add("RotateLeft", ROTATELEFT);
            CONTROLS.Add("Forcedrop", FORCEDROP);
            CONTROLS.Add("Hold", HOLD);
        }
    }

    public struct AudioSettings
    {
        public static int VOL;
        public static int MUSIC;
        public static readonly Dictionary<string, int> AUDIO = new Dictionary<string, int>();

        public AudioSettings(int volume, int music)
        {
            VOL = volume;
            MUSIC = music;
            createDic();
        }

        private void createDic()
        {
            AUDIO.Clear();
            AUDIO.Add("Volume", VOL);
            AUDIO.Add("Music", MUSIC);
        }
    }
}
