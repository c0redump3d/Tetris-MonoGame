using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris.Other;

namespace Tetris.GUI
{
    public class GuiDebug
    {
        
        private List<ChatLine> consoleMessageList;
        private List<string> debugOptions;
        private List<bool> debugEnabled;
        private int cursorPos = 0;
        private KeyboardState keyState;
        private KeyboardState oldKeyState;
        private bool showDebugMenu = false;
        private string menuText = $"Tetris Debug Menu";
        private float longestTextX = 0f;
        private float longestTextY = 0f;

        public GuiDebug()
        {
            consoleMessageList = new List<ChatLine>();
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

            if (showDebugMenu)
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
            {
                CheckLength();
            }
        }
        
        public void DrawDebugConsole(SpriteBatch spriteBatch)
        {
            if (IsOptionEnabled(0))
                DrawConsole(spriteBatch);
        }

        public void DrawDebugMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (showDebugMenu)
            {
                int lastY = 0;
                spriteBatch.Draw(Instance.DebugBox, new Rectangle(0,0, (int)longestTextX+2, (debugOptions.Count+1)*12 + 20), Color.Black * 0.5f);
                spriteBatch.DrawString(Globals.ConsoleFont, $"{menuText}", new Vector2(0, 0),
                    Color.White);
                for (int i = 0; i < debugOptions.Count; i++)
                {
                    string cursor = cursorPos == i ? ">" : "";
                    string enabled = debugEnabled[i] ? "ON" : "OFF";
                    spriteBatch.DrawString(Globals.ConsoleFont, $"{cursor}{debugOptions[i]}:{enabled}",
                        new Vector2(10, (i + 1) * 12), Color.White);
                    lastY = (i + 1) * 12;
                }
                string extraInfo =
                    $"{Globals.Version} fps: {(1 / gameTime.ElapsedGameTime.TotalSeconds):n2} os: {Globals.CurrentOS}";
                
                spriteBatch.DrawString(Globals.ConsoleFont, $"{extraInfo}", new Vector2(0, lastY+18), Color.White);
            }
        }

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
            int lines = 25;
            
            for(int msg = 0; msg < consoleMessageList.Count && msg < lines; msg++)
            {
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
        
        public void AddConsoleMessage(string s)
        {
            int i;
            int spacing = 230;
            for(; Globals.ConsoleFont.MeasureString(s).X > spacing; s = s.Substring(i))
            {
                for(i = 1; i < s.Length && Globals.ConsoleFont.MeasureString(s.Substring(0, i + 1)).X <= spacing; i++) { }
                AddConsoleMessage(s.Substring(0, i));
            }

            consoleMessageList.Insert(0, new ChatLine(s));
            for(; consoleMessageList.Count > 50; consoleMessageList.RemoveAt(consoleMessageList.Count - 1)) { }
        }
        
        // private void UpdateConsole()
        // {
        //     for(int i = 0; i < consoleMessageList.Count; i++)
        //     {
        //         consoleMessageList[i].UpdateCounter++;
        //     }
        // }

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
        }
        
    }
}