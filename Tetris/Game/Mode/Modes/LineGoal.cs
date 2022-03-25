using Microsoft.Xna.Framework;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;

namespace Tetris.Game.Mode.Modes;

public class LineGoal : CustomMode
{
    public LineGoal(string name, string objective) : base(name, objective)
    {
    }

    public override void SetUp()
    {
        base.SetUp();
        //stop player from being able to level up
        EventManager.Instance.GetEvent("levelup").SetCancelled(true);
        //re-enable when game ends.
        AddPreEvent("endgame", "allowlevelup", () => 
            EventManager.Instance.GetEvent("levelup").SetCancelled(false));
    }

    public override void Update(GameTime gameTime)
    {
        if (InGameManager.Instance.Paused)
            return;
        if (ScoreHandler.Instance.TotalLines >= 40 &&
            InGameManager.Instance.CanMove)
        {
            InGameManager.Instance.Winner = true;
            //Call the player death event to end the game.
            EventManager.Instance.GetEvent("playerdeath").Call();
        }

        base.Update(gameTime);
    }
}