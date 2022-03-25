using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Mode.Modes;

namespace Tetris.Game.Mode;

public class ModeManager
{
    private static ModeManager _instance;
    public readonly List<CustomMode> GameModes;

    private CustomMode currentMode;

    private ModeManager()
    {
        GameModes = new List<CustomMode>();
        RegisterModes();
        SetCurrentMode("Survival");
    }

    public static ModeManager Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new ModeManager();

            return result;
        }
    }

    /// <summary>
    ///     Registers all game modes that are accessible in the game.
    /// </summary>
    private void RegisterModes()
    {
        GameModes.Add(new CustomMode("Survival", "Survive as long as possible!"));
        GameModes.Add(new TimeTrial("Time Trial", "Clear as many lines as possible in 3 minutes!"));
        GameModes.Add(new LineGoal("40 Line", "Race to clear 40 lines!"));
        GameModes.Add(new Hardcore("Hardcore", "Survive and get as many tetrises and t-spins as possible! Also, no holding tetriminoes!"));
        GameModes.Add(new Classic("Classic", "A mode closely resembling the old tetris play style! No holding or hard drop, " +
                                             "just classic tetris! Includes custom gravity intervals!"));
        //GameModes.Add(new Challenge("Challenge", "   Complete the given challenge\nor be punished if you're too slow!"));
    }

    /// <summary>
    ///     Sets the game mode to the provided input.
    /// </summary>
    /// <param name="name">Name of game mode</param>
    public void SetCurrentMode(string name)
    {
        foreach (var mode in GameModes)
            if (mode.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                currentMode = mode;
    }

    public CustomMode GetCurrentMode()
    {
        return currentMode;
    }

    public void UpdateCurrentMode(GameTime gameTime)
    {
        currentMode.Update(gameTime);
    }

    public void DrawCurrentMode(SpriteBatch spriteBatch)
    {
        currentMode.Draw(spriteBatch);
    }
}