using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.GUI;

namespace Tetris.Game.Mode;

/// <summary>
///     Used to easily implement new game modes!
/// </summary>
public class CustomMode
{
    public CustomMode(string name, string objective)
    {
        Name = name;
        Objective = objective;
    }
    
    /// <summary>
    ///     Name of the game mode.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     A basic description of what the player should do in this mode.
    /// </summary>
    public string Objective { get; }

    /// <summary>
    ///     If true, the pinch overlay will be drawn.
    /// </summary>
    public bool ShowPinch { get; set; }

    /// <summary>
    ///     Keeps track of events added by the custom mode(So that they're removed when game ends).
    /// </summary>
    private List<string> AddedEvents;

    //TODO: Implement a proper EventHandling system.
    //Ideally later it would be better to implement a proper event system.

    public virtual void SetUp()
    {
        OnGameStart();
        AddedEvents = new();
        AddPostEvent("endgame", $"{Name.ToLower()}cleanup", RemoveEvents);
    }
    
    protected virtual void OnGameStart()
    {
        ShowPinch = false;
    }

    protected void AddEvent(string name, Action func)
    {
        AddedEvents.Add(name);
        EventManager.Instance.CreateEvent(name, func);
    }
    
    /// <summary>
    ///     Adds a post event to the parent event.
    /// </summary>
    protected void AddPostEvent(string parent, string name, Action func)
    {
        AddedEvents.Add(name);
        EventManager.Instance.GetEvent(parent).AddPostEvent(name, func);
    }
    
    /// <summary>
    ///     Adds a pre event to the parent event.
    /// </summary>
    protected void AddPreEvent(string parent, string name, Action func)
    {
        AddedEvents.Add(name);
        EventManager.Instance.GetEvent(parent).AddPreEvent(name, func);
    }
    
    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    private void RemoveEvents()
    {
        if (this == ModeManager.Instance.GetCurrentMode())
        {
            foreach (string ev in AddedEvents)
                EventManager.Instance.RemoveEvent(ev);
            EventManager.Instance.RemoveEvent($"{Name.ToLower()}cleanup");
            Gui.Instance.AddDebugMessage(
                $"Removed {Name} mode post & pre events from EventManager({AddedEvents.Count}).");
        }
    }
}