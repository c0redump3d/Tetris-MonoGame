using System;
using System.Collections.Generic;
using System.Linq;
using Tetris.GUI;

namespace Tetris.Game.Events;

public class Event
{
    private bool Cancelled { get; set; }
    /// <summary>
    ///     The name identifier of the event.
    /// </summary>
    public string Name { get; }

    private Action<object[]?> ToRun { get; }
    /// <summary>
    ///     This event is invoked after the event has been run.
    /// </summary>
    private Action RunPost { get; }
    /// <summary>
    ///     This event is invoked before the event has been run.
    /// </summary>
    private Action RunPre { get; }
    /// <summary>
    ///     Contains a string identifier that points to an event to be run before the event is run.
    /// </summary>
    public List<Event> PreEvent { get; }
    /// <summary>
    ///     Contains a string identifier that points to an event to be run after the event is run.
    /// </summary>
    public List<Event> PostEvent { get; }

    public Event(string name, Action<object[]?> func)
    {
        Name = name;
        PreEvent = new();
        PostEvent = new();
        ToRun += func;
        RunPre += () =>
        {
            foreach (Event ev in PreEvent.ToList()) ev?.Call();
        };
        RunPost += () =>
        {
            foreach (Event ev in PostEvent.ToList()) ev?.Call();
        };
    }

    public void Call(object[] args = null)
    {
        RunPre?.Invoke();
        if (!IsCancelled())
            ToRun?.Invoke(args);
        RunPost?.Invoke();
    }
    
    public void Call()
    {
        Call(null);
    }

    public void AddPreEvent(string name, Action<object[]> func)
    {
        PreEvent.Add(new Event(name, func));
        Gui.Instance.AddDebugMessage($"Added pre event {name} to parent event {Name}.");
    }
    
    public void AddPreEvent(string name, Action func)
    {
        void Converted(object[] o) => func?.Invoke();
        PreEvent.Add(new Event(name, Converted));
        Gui.Instance.AddDebugMessage($"Added pre event {name} to parent event {Name}.");
    }
    
    public void AddPostEvent(string name, Action func)
    {
        void Converted(object[] o) => func?.Invoke();
        PostEvent.Add(new Event(name, Converted));
        Gui.Instance.AddDebugMessage($"Added post event {name} to parent event {Name}.");
    }
    
    public void AddPostEvent(string name, Action<object[]> func)
    {
        PostEvent.Add(new Event(name, func));
        Gui.Instance.AddDebugMessage($"Added post event {name} to parent event {Name}.");
    }

    public bool RemovePreEvent(string name)
    {
        foreach (var pre in PreEvent)
        {
            if (pre.Name.ToLower().Equals(name.ToLower()))
            {
                PreEvent.Remove(pre);
                Gui.Instance.AddDebugMessage($"Removed pre event {name} from parent event {Name}.");
                return true;
            }
        }
        //Gui.Instance.AddDebugMessage($"Didn't remove pre event {name} from parent {Name} (child event does not exist).");
        return false;
    }

    public bool RemovePostEvent(string name)
    {
        foreach (var post in PostEvent)
        {
            if (post.Name.ToLower().Equals(name.ToLower()))
            {
                PostEvent.Remove(post);
                Gui.Instance.AddDebugMessage($"Removed post event {name} from parent event {Name}.");
                return true;
            }
        }
        //Gui.Instance.AddDebugMessage($"Didn't remove post event {name} from parent {Name} (child event does not exist).");
        return false;
    }
    
    /// <summary>
    ///     Returns whether or not the event is to be cancelled.
    /// </summary>
    public bool IsCancelled()
    {
        return Cancelled;
    }

    /// <summary>
    ///     Sets whether or not the event should be cancelled or not.
    /// </summary>
    public void SetCancelled(bool cancel)
    {
        Gui.Instance.AddDebugMessage($"Cancel update ({cancel.ToString()}) for event {Name}.");
        Cancelled = cancel;
    }
}