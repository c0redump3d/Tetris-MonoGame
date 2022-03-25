using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Util;

namespace Tetris.GUI.Control.Controls.DebugControls;

/// <summary>
///     A very basic line graph to see framerate over a period of 15 seconds.
/// </summary>
public class FpsGraph : UiControl
{
    private readonly FrameCounter counter;
    private readonly List<float> fpsPoints;

    private float fpsTick;
    private Vector2 prevTick;

    public FpsGraph(int x, int y)
    {
        counter = new FrameCounter();
        fpsPoints = new List<float>();
        fpsPoints.AddRange(new float[15]);
        Position = new Vector2(x, y);
        Size = new Vector2(150, 60);
        prevTick = new Vector2(Position.X, Position.Y + Size.Y);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var count = 0;
        spriteBatch.DrawString(Fonts.ConsoleFont, "60", new Vector2(Position.X - 16, Position.Y - 5), Color.White);
        spriteBatch.DrawString(Fonts.ConsoleFont, "0", new Vector2(Position.X - 12, Position.Y + Size.Y - 10),
            Color.White);
        //Draw the current fps and average framerate of the game below the graph.
        spriteBatch.DrawCenteredString(Fonts.ConsoleFont,
            $"FPS: {counter.CurrentFramesPerSecond:n2} Avg: {counter.AverageFramesPerSecond:n2}",
            new Vector2(Position.X + Size.X / 2f, Position.Y + Size.Y + 10), Color.White);
        //Vertical line of the graph.
        spriteBatch.DrawLine(new Vector2(Position.X, Position.Y), new Vector2(Position.X, Position.Y + Size.Y),
            Color.Gray);
        //Horizontal line of the graph.
        spriteBatch.DrawLine(new Vector2(Position.X, Position.Y + Size.Y),
            new Vector2(Position.X + Size.X, Position.Y + Size.Y), Color.Gray);
        foreach (var pnt in fpsPoints)
        {
            var newVec = new Vector2(Position.X + count * 10, Position.Y + Size.Y - Math.Clamp(pnt, 0f, Size.Y));
            //first index isn't drawn as it is removed often, so just set prevTick to what it would be.
            if (count == 0)
            {
                prevTick = newVec;
                count++;
                continue;
            }

            //Set the previous ticks position to behind the current tick.
            prevTick.X = Position.X + count * 10 - 10;
            //Draws the line
            spriteBatch.DrawLine(prevTick, newVec, Color.Red);
            prevTick = newVec;
            count++;
        }

        base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        //The delta time is the elapsed time in seconds.
        var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
        //Update the FramerateCounter.
        counter.Update(deltaTime);

        fpsTick -= gameTime.ElapsedGameTime.Milliseconds;
        if (fpsTick <= 0)
        {
            //add new tick to graph.
            fpsPoints.Add(counter.CurrentFramesPerSecond);
            //if we are over 15 total ticks, remove oldest tick.
            if (fpsPoints.Count > 16)
                fpsPoints.RemoveAt(0);
            //wait 1 second before repeating.
            fpsTick = 1000;
        }

        base.Update(gameTime);
    }
}