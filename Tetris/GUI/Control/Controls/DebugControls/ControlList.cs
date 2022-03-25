using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls.DebugControls;

/// <summary>
///     Generates and draws a list of all of the current controls on-screen.
/// </summary>
public class ControlList : UiControl
{
    private List<string> controlString;

    public ControlList()
    {
        controlString = new List<string>();
        Size = new Vector2(100, 50);
    }

    public override void Update(GameTime gameTime)
    {
        var controls = Gui.Instance.CurrentScreen.Controls;
        float longestWidth = 0;
        controlString = new List<string>();
        //Loops through all of the current screens controls.
        foreach (var con in controls)
        {
            var txt =
                $"{con.ID}:{con.GetType().Name}:Pos[{(int) con.Position.X},{(int) con.Position.Y}]," +
                $"Size[{(int) con.Size.X}x{(int) con.Size.Y}]\nEnabled:{con.Enabled},Hover:{con.Hover}";
            controlString.Add(txt);
            //Update longestWidth to allow the panel to fit the text.
            if (longestWidth < Fonts.ConsoleFont.MeasureString(txt).X)
                longestWidth = Fonts.ConsoleFont.MeasureString(txt).X;
        }

        //Update the panel size.
        Size = new Vector2(longestWidth, 30 * controls.Count);
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var count = 0;
        spriteBatch.DrawCenteredString(Fonts.ConsoleFont, $"Controls ({Gui.Instance.CurrentScreen.GetType().Name}):",
            new Vector2(Position.X + Size.X / 2f, Position.Y + 5), Color.White);
        foreach (var text in controlString)
        {
            //Makes it easier to point out what control is currently being hovered.
            var hovered = text.ToLower().Contains("hover:true");
            spriteBatch.DrawString(Fonts.ConsoleFont, text, new Vector2(Position.X, Position.Y + 15 + 30 * count),
                hovered ? Color.Red : Color.White);
            count++;
        }

        base.Draw(spriteBatch);
    }
}