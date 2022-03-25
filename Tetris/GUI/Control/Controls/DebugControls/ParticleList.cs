using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.GUI.Particle;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls.DebugControls;

/// <summary>
///     Generates and draws a list of all of the current particles on-screen.
/// </summary>
public class ParticleList : UiControl
{
    private List<string> particleString;

    public ParticleList()
    {
        particleString = new List<string>();
        Size = new Vector2(100, 50);
    }

    public override void Update(GameTime gameTime)
    {
        var activeParticles = ParticleManager.Instance.ActiveParticles;
        float longestWidth = 0;
        var count = 0;
        particleString = new List<string>();
        //Loops through all of the current particles.
        foreach (var particle in activeParticles)
        {
            var txt = $"{count}:Pos[{particle.X:n2},{particle.Y:n2}],Size:{particle.Size:n2}" +
                      $"\nLifeSpan:{particle.LifeSpan:n2},Alive:{particle.Alive}";
            particleString.Add(txt);
            //Update longestWidth to allow the panel to fit the text.
            if (longestWidth < Fonts.ConsoleFont.MeasureString(txt).X)
                longestWidth = Fonts.ConsoleFont.MeasureString(txt).X;
            count++;
        }

        //Update the panel size.
        Size = new Vector2(longestWidth, 30 * activeParticles.Count);
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var count = 0;
        spriteBatch.DrawCenteredString(Fonts.ConsoleFont, "Particle List",
            new Vector2(Position.X + Size.X / 2f, Position.Y + 5), Color.White);
        foreach (var text in particleString)
        {
            //Makes it easier to point out what control is currently being hovered.
            var hovered = text.ToLower().Contains("alive:false");
            spriteBatch.DrawString(Fonts.ConsoleFont, text, new Vector2(Position.X, Position.Y + 15 + 30 * count),
                hovered ? Color.Red : Color.White);
            count++;
        }

        base.Draw(spriteBatch);
    }
}