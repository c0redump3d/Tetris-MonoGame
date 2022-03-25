using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls;

public class Paragraph : UiControl
{
    private List<string> FormattedString { get; set; }
    private Vector2 origPos;
    private float Spacing { get; }
    private string currentText;
    
    /// <summary>
    ///     Creates an auto-formatted paragraph that is centered around the X,Y position.
    /// </summary>
    public Paragraph(string text, SpriteFont font, Vector2 position, float spacing = 300)
    {
        Text = text;
        currentText = Text;
        Font = font;
        FormattedString = new List<string>();
        Size = new Vector2(150, 50);
        origPos = position;
        Spacing = spacing;
        LayoutText();
    }

    private void LayoutText()
    {
        FormattedString = new List<string>();
        currentText = Text;
        int i;
        string s = Text;
        //split any text that exceeds out spacing
        for (; Font.MeasureString(s).X > Spacing; s = s.Substring(i)) 
        {
            for (i = 1; i < s.Length && Font.MeasureString(s.Substring(0, i + 1)).X <= Spacing; i++)
            {
            }
            FormattedString.Add(s.Substring(0, i));
        }

        FormattedString.Add(s);
        UpdateSize();
    }

    private void UpdateSize()
    {
        float longestX = 0, longestY = 0;
        int count = 0;
        foreach (string line in FormattedString)
        {
            if (Font.MeasureString(line).X > longestX)
                longestX = Font.MeasureString(line).X;
        }
        Size = new Vector2(longestX, ((FormattedString.Count)*Font.LineSpacing));
        Position = new Vector2(origPos.X - (Size.X/2f), origPos.Y - (Size.Y/2f));
    }
    
    public override void Update(GameTime gameTime)
    {
        if (currentText != Text)
            LayoutText();

        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        int count = 0;
        Vector2 pos = new Vector2(Position.X + (Size.X/2f), Position.Y + Font.LineSpacing/2f);
        foreach (string line in FormattedString)
        {
            spriteBatch.DrawCenteredString(Font, line, pos, Color.White * Gui.Instance.CurrentScreen.Opacity);
            count++;
            pos = new Vector2(Position.X + (Size.X/2f), (Position.Y + Font.LineSpacing/2f) + (Font.LineSpacing * (count)));
        }
        base.Draw(spriteBatch);
    }
}