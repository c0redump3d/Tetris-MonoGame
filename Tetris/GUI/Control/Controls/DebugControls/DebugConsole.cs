using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls.DebugControls;

/// <summary>
///     Console to help print useful information when developing new features & fixing bugs in-game.
/// </summary>
public class DebugConsole : UiControl
{
    private readonly List<MessageLine> consoleMessageList;

    public DebugConsole()
    {
        consoleMessageList = new List<MessageLine>();
        Size = new Vector2(291, 0);
        Position = Vector2.Zero;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!DebugMenu.DebugMenu.Instance.IsOptionEnabled(0))
            return;

        var lines = 40; // how many messages can be on screen
        var spacing = 0;
        var total = consoleMessageList.Count * 12;
        var rec = new Rectangle((int) Position.X, (int) (Position.Y + Size.Y) - total, 291, total);
        //spriteBatch.DrawBorderedRect(rec, new Color(30, 28, 28) * 0.8f,
        //new Color(43, 149, 223));
        Size = new Vector2(rec.Width, rec.Height);
        //spriteBatch.DrawCenteredString(Fonts.ConsoleFont, $"Tetris {Globals.Version} Debug Console",
        //new Vector2(Position.X + (Size.X/2f), (Position.Y+Size.Y) - total), Color.White);
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
                spriteBatch.DrawString(Fonts.ConsoleFont, s1, new Vector2(561, 660 + spacing), Color.White * opac);
            }*/
            spacing = msg * 12;
            var s1 = consoleMessageList[msg].Message;
            spriteBatch.DrawString(Fonts.ConsoleFont, s1, new Vector2(Position.X + 2, Position.Y + Size.Y - spacing),
                Color.White);
        }

        spriteBatch.DrawCenteredString(Fonts.ConsoleFont, "Console",
            new Vector2(Position.X + Size.X / 2f, Position.Y + 10), Color.White);
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
             Fonts.ConsoleFont.MeasureString(s).X > spacing;
             s = s.Substring(i)) //split any text that exceeds out spacing
        {
            for (i = 1; i < s.Length && Fonts.ConsoleFont.MeasureString(s.Substring(0, i + 1)).X <= spacing; i++)
            {
            }

            AddConsoleMessage(s.Substring(0, i));
        }

        consoleMessageList.Insert(0, new MessageLine(s));
        for (; consoleMessageList.Count > 40; consoleMessageList.RemoveAt(consoleMessageList.Count - 1))
        {
        } // any lines that exceed 50 will be removed
    }

    public void AddMessage(string message, [CallerMemberName] string caller = "")
    {
        if (DebugMenu.DebugMenu.Instance.IsOptionEnabled(0))
            AddConsoleMessage(caller == null ? $@"{message}" : $@"{caller}: {message}");
    }
}