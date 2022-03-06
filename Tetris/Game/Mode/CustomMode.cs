using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.Game.Mode
{
    /// <summary>
    /// Used to easily implement new game modes!
    /// </summary>
    public class CustomMode
    {
        /// <summary>
        /// Name of the game mode.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// A basic description of what the player should do in this mode.
        /// </summary>
        public string Objective { get; }
        /// <summary>
        /// If true, the pinch overlay will be drawn.
        /// </summary>
        public bool ShowPinch { get; set; }
        /// <summary>
        /// (Default value is true)
        /// If false, the player will be unable to level up past the level they started on.
        /// </summary>
        public bool LevelUp { get; set; } = true;

        public CustomMode(string name, string objective)
        {
            Name = name;
            Objective = objective;
        }

        //TODO: Implement a proper EventHandling system.
        //Ideally later it would be better to implement a proper event system.
        
        public virtual void OnGameStart()
        {
            ShowPinch = false;
        }

        /// <summary>
        /// Called whenever a row has started to be removed.
        /// </summary>
        public virtual void OnRowRemove()
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}