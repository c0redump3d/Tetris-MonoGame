using System.Collections.Generic;

namespace Tetris.GUI
{
    public class GuiDebug
    {
<<<<<<< Updated upstream
        //TODO
        private List<string> debugMessages = new(6);
        private int count;
        public bool Enabled { get; set; }
=======
        
        private List<DebugLine> consoleMessageList;
        private List<string> debugOptions;
        private List<bool> debugEnabled;
        private int cursorPos = 0;
        private KeyboardState keyState;
        private KeyboardState oldKeyState;
        private bool showDebugMenu = false;
        private string menuText = $"Tetris Debug Menu";
        private float longestTextX = 0f;
        private float longestTextY = 0f;
>>>>>>> Stashed changes

        public void PrintMessage(string message)
        {
<<<<<<< Updated upstream
            if (count == 6)
=======
            consoleMessageList = new List<DebugLine>();
            debugOptions = new List<string>();
            debugEnabled = new List<bool>();
            AddOption("Debug Console");
            AddOption("Tetrimino Names");
            AddOption("Tetrimino Info");
            AddOption("Draw Rotation Tetriminos");
            AddOption("Button Debug");
            AddOption("Tetrimino Mouse Editor");
        }

        private void AddOption(string name)
        {
            debugOptions.Add(name);
            debugEnabled.Add(false);
        }
        
        public void Update()
        {
            //UpdateConsole();

            if (showDebugMenu) // listen for arrow keypresses to update debug menu
            {
                keyState = Keyboard.GetState();

                if (IsKeyPress(Keys.Down))
                {
                    if (cursorPos < debugOptions.Count - 1)
                        cursorPos++;
                    else
                        cursorPos = 0;

                    CheckLength();
                }

                if (IsKeyPress(Keys.Up))
                {
                    if (cursorPos > 0)
                        cursorPos--;
                    else
                        cursorPos = debugOptions.Count - 1;
                    CheckLength();
                }

                if (IsKeyPress(Keys.Enter))
                {
                    debugEnabled[cursorPos] = !debugEnabled[cursorPos];
                    CheckLength();
                }

                oldKeyState = keyState;
            }
        }

        public void ShowMenu()
        {
            showDebugMenu = !showDebugMenu;
            if (showDebugMenu)
>>>>>>> Stashed changes
            {
                MoveUp();
                count = 0;
            }

            debugMessages[count] = message;
            count++;
        }

<<<<<<< Updated upstream
        private void MoveUp()
        {
            debugMessages[0] = debugMessages[1];
            debugMessages[1] = debugMessages[2];
            debugMessages[3] = debugMessages[4];
            debugMessages[4] = debugMessages[5];
=======
        /// <summary>
        /// This checks to see which debug option has the longest text so that the bordered rect encases everything
        /// </summary>
        private void CheckLength()
        {
            longestTextX = Globals.ConsoleFont.MeasureString(menuText).X;
            longestTextY = Globals.ConsoleFont.MeasureString(menuText).Y;

            for (int i = 0; i < debugOptions.Count; i++)
            {
                string cursor = cursorPos == i ? ">" : "";
                string enabled = debugEnabled[i] ? "ON" : "OFF";
                string text = $"{cursor}{debugOptions[i]}:{enabled}";
                if (Globals.ConsoleFont.MeasureString(text).X + 10 > longestTextX)
                    longestTextX = Globals.ConsoleFont.MeasureString(text).X + 10;
                if (Globals.ConsoleFont.MeasureString(text).Y > longestTextY)
                    longestTextY = Globals.ConsoleFont.MeasureString(text).Y;
            }
        }
        
        private void DrawConsole(SpriteBatch spriteBatch)
        {
            int lines = 25; // how many messages can be on screen
            
            for(int msg = 0; msg < consoleMessageList.Count && msg < lines; msg++)
            {
                //Message fade is not going to be used for this, but will save code incase
                /*if(consoleMessageList[msg].UpdateCounter >= 200)
                {
                    continue;
                }
                float opac = consoleMessageList[msg].UpdateCounter / 200.0f;
                opac = 1.0f - opac;
                opac *= 10f;
                if(opac < 0.0f)
                {
                    opac = 0.0f;
                }
                if(opac > 1.0f)
                {
                    opac = 1.0f;
                }
                opac *= opac;
                if(opac > 0)
                {
                    int spacing = -msg * 12;
                    string s1 = consoleMessageList[msg].Message;
                    spriteBatch.DrawString(Globals.ConsoleFont, s1, new Vector2(561, 660 + spacing), Color.White * opac);
                }*/
                int spacing = -msg * 12;
                string s1 = consoleMessageList[msg].Message;
                spriteBatch.DrawString(Globals.ConsoleFont, s1, new Vector2(561, 665 + spacing), Color.White);
            }
            
        }

        public void DebugMessage(string message, [CallerMemberName] string caller = "")
        {
            if(IsOptionEnabled(0))
                AddConsoleMessage($@"{caller}: {message}");
        }

        private void AddConsoleMessage(string s)
        {
            int i;
            int spacing = 230;
            for(; Globals.ConsoleFont.MeasureString(s).X > spacing; s = s.Substring(i)) //split any text that exceeds out spacing
            {
                for(i = 1; i < s.Length && Globals.ConsoleFont.MeasureString(s.Substring(0, i + 1)).X <= spacing; i++) { }
                AddConsoleMessage(s.Substring(0, i));
            }

            consoleMessageList.Insert(0, new DebugLine(s));
            for(; consoleMessageList.Count > 50; consoleMessageList.RemoveAt(consoleMessageList.Count - 1)) { } // any lines that exceed 50 will be removed
        }
        
        //Again, this is for message fading, not being used
        // private void UpdateConsole()
        // {
        //     for(int i = 0; i < consoleMessageList.Count; i++)
        //     {
        //         consoleMessageList[i].UpdateCounter++;
        //     }
        // }

        /// <summary>
        /// Check if a debug option is currently enabled or disabled.
        /// </summary>
        /// <param name="mod">Debug Option Index</param>
        /// <returns>True if debug option is enabled</returns>
        public bool IsOptionEnabled(int mod)
        {
            if (mod < 0 || mod > debugOptions.Count - 1)
                return false;
            
            return debugEnabled[mod];
        }
        
        private bool IsKeyPress(Keys key)
        {
            if (keyState.IsKeyDown(key) && !oldKeyState.IsKeyDown(key))
                return true;

            return false;
>>>>>>> Stashed changes
        }
    }
}