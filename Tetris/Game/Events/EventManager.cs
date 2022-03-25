using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tetris.GUI;

namespace Tetris.Game.Events;

public class EventManager
{
    private static EventManager _instance;
    private List<Event> EventList { get; }

    private EventManager()
    {
        EventList = new List<Event>();
    }

    public static EventManager Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new EventManager();

            return result;
        }
    }

    public void CreateEvent(string name, Action<object[]> func)
    {
        if (EventList.Any(ev => ev.Name.ToLower().Equals(name.ToLower())))
        {
            Gui.Instance.AddDebugMessage($"Failed to create event {name} as it already exists!");
            return;
        }

        EventList.Add(new Event(name, func));
        Gui.Instance.AddDebugMessage($"Created new event with name {name}.");
    }
    
    public void CreateEvent(string name, Action func)
    {
        var converted = new Action<object[]>(o=>func?.Invoke());
        CreateEvent(name, converted);
    }

    public void RemoveEvent(string name)
    {
        foreach (Event ev in EventList)
        {
            //Make sure event does not already exist.
            if (ev.Name.ToLower().Equals(name.ToLower()))
            {
                EventList.Remove(ev);
                Gui.Instance.AddDebugMessage($"Removed event {name}.");
            }
            else
            {
                if (!ev.RemovePreEvent(name))
                {
                    if(ev.RemovePostEvent(name))
                        Gui.Instance.AddDebugMessage($"Removed post event {name}.");
                }
                else
                {
                    Gui.Instance.AddDebugMessage($"Removed pre event {name}.");
                }
            }
        }
    }

    /// <summary>
    ///     Finds and returns an event with the given name.
    /// </summary>
    public Event GetEvent(string name)
    {
        foreach (Event ev in EventList)
        {
            if (ev.Name.ToLower().Equals(name.ToLower()))
            {
                return ev;
            }
            
            foreach (var pre in ev.PreEvent.Where(pre => pre.Name.ToLower().Equals(name.ToLower())))
            {
                return pre;
            }

            foreach (var post in ev.PostEvent.Where(post => post.Name.ToLower().Equals(name.ToLower())))
            {
                return post;
            }
        }

        Gui.Instance.AddDebugMessage($"Unable to find event {name}.");
        return null;
    }
}