using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.GUI.DebugMenu;

namespace Tetris.Util
{
    public class TimerVar
    {
        public TimerVar(int id, string name, float time, Action func)
        {
            ID = id;
            Name = name;
            Time = time;
            Function = func;
        }

        public int ID { get; }
        public string Name { get; }
        public float Time { get; set; }
        public Action Function { get; }
        public event EventHandler Tick;
        public void OnTick(EventArgs e)
        {
            EventHandler handler = Tick;
            handler?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Allows for delayed code functionality and other extremely useful features,
    /// EX: this is used multiple times in the row fall function to allow for a animation to play out before the row actually falls.
    /// </summary>
    public class TimerUtil
    {
        private static TimerUtil _instance;
        public static TimerUtil Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new TimerUtil();
                }

                return result;
            }
        }

        //Allow for multiple timers to be created at once.
        private readonly List<TimerVar> timers = new();
        private int totalID = -1;

        private TimerUtil()
        {
            timers = new List<TimerVar>();
        }

        /// <summary>
        ///     Creates a timer with the specified time in milliseconds.
        /// </summary>
        /// <param name="time">Time in milliseconds</param>
        /// <param name="runFunc">Function to run after timer has completed (Optional)</param>
        public void CreateTimer(float time, Action runFunc = null, string name = "")
        {
            //If a timer name is specified, we assume we do not want another of the same timer running concurrently!
            if (!name.Equals(""))
                foreach (var timer in timers)
                    if (name.Equals(timer.Name) && timer.Time >= 0)
                    {
                        DebugConsole.Instance.AddMessage($"Timer with name {name} already created!");
                        return;
                    }

            totalID++;
            timers.Add(new TimerVar(totalID, name, time, runFunc));
        }
        
        /// <summary>
        ///     Creates a timer with the specified time in milliseconds.
        /// </summary>
        /// <param name="time">Time in milliseconds</param>
        public void CreateTimer(float time, string name = "")
        {
            if (!name.Equals(""))
                foreach (var timer in timers)
                    if (name.Equals(timer.Name) && timer.Time >= 0)
                    {
                        DebugConsole.Instance.AddMessage($"Timer with name {name} already created!");
                        return;
                    }

            totalID++;
            timers.Add(new TimerVar(totalID, name, time, null));
        }

        public void UpdateTimers(GameTime gameTime)
        {
            //if we have no timers, don't waste any time
            if (timers.Count <= 0 || InGameManager.Instance.Paused)
                return;

            //subtract from our total time for active timer
            for (var i = 0; i < timers.Count; i++)
                if (timers[i].Time >= 0)
                {
                    //Raise timer tick event
                    timers[i].OnTick(new EventArgs());
                    timers[i].Time -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    //run our function after timer has successfully completed.
                    if(timers[i].Function!=null)
                        timers[i].Function.Invoke();
                    //now, remove the timer
                    timers.RemoveAt(i);
                    totalID--;
                }
        }

        /// <summary>
        /// Allows access to a specific timer if name is provided
        /// </summary>
        /// <param name="name">Name of timer</param>
        /// <returns>TimerUtil</returns>
        public TimerVar GetTimer(string name)
        {
            foreach (var timer in timers)
                if (timer.Name.Equals(name))
                    return timer;

            DebugConsole.Instance.AddMessage($"Unable to find timer with name: {name}");
            return null;
        }
    }
}