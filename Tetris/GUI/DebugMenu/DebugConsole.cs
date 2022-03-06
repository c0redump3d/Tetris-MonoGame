using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Control.Controls;
using Tetris.Util;

namespace Tetris.GUI.DebugMenu
{
    /// <summary>
    /// Console to help print useful information when developing new features & fixing bugs in-game.
    /// </summary>
    public class DebugConsole
    {
        
        private readonly List<MessageLine> consoleMessageList;

        private DebugConsole()
        {
            consoleMessageList = new List<MessageLine>();
        }
        
        private void DrawConsole(SpriteBatch spriteBatch)
        {
            var lines = 52; // how many messages can be on screen
            var spacing = 0;
            var total = consoleMessageList.Count * 12;

            spriteBatch.Begin();
            spriteBatch.DrawBorderedRect(new Rectangle(984, 655 - total, 291, 40 + total), new Color(30, 28, 28) * 0.8f,
                new Color(43, 149, 223));
            spriteBatch.DrawString(Globals.ConsoleFont, $"Tetris {Globals.Version} Debug Console",
                new Vector2(985, 655 - total), Color.White);
            for (var msg = 0; msg < consoleMessageList.Count && msg < lines; msg++)
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
                spacing = -msg * 12;
                var s1 = consoleMessageList[msg].Message;
                spriteBatch.DrawString(Globals.ConsoleFont, s1, new Vector2(985, 680 + spacing), Color.White);
            }

            spriteBatch.End();
        }
        
        //Again, this is for message fading, not being used
        // private void UpdateConsole()
        // {
        //     for(int i = 0; i < consoleMessageList.Count; i++)
        //     {
        //         consoleMessageList[i].UpdateCounter++;
        //     }
        // }

        private void AddConsoleMessage(string s)
        {
            int i;
            var spacing = 290;
            for (;
                Globals.ConsoleFont.MeasureString(s).X > spacing;
                s = s.Substring(i)) //split any text that exceeds out spacing
            {
                for (i = 1; i < s.Length && Globals.ConsoleFont.MeasureString(s.Substring(0, i + 1)).X <= spacing; i++)
                {
                }

                AddConsoleMessage(s.Substring(0, i));
            }

            consoleMessageList.Insert(0, new MessageLine(s));
            for (; consoleMessageList.Count > 52; consoleMessageList.RemoveAt(consoleMessageList.Count - 1))
            {
            } // any lines that exceed 50 will be removed
        }

        public void AddMessage(string message, [CallerMemberName] string caller = "")
        {
            if (DebugMenu.Instance.IsOptionEnabled(0)) AddConsoleMessage(caller == null ? $@"{message}" : $@"{caller}: {message}");
        }
        
        public void DrawDebugConsole(SpriteBatch spriteBatch)
        {
            if (DebugMenu.Instance.IsOptionEnabled(0))
                DrawConsole(spriteBatch);
        }

        private static DebugConsole _instance;
        public static DebugConsole Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new DebugConsole();
                }

                return result;
            }
        }
    }
}