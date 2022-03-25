using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game.Events;
using Tetris.Game.InGame;
using Tetris.Game.Managers;
using Tetris.Game.Player;
using Tetris.GUI;

namespace Tetris.Game.Mode.Modes;

public class TimeTrial : CustomMode
{
    private int minutesRemaining = 2;
    private int secondsRemaining = 59;
    private int timerWait = 1000;

    public TimeTrial(string name, string objective) : base(name, objective)
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

    protected override void OnGameStart()
    {
        minutesRemaining = 2;
        secondsRemaining = 59;
        base.OnGameStart();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var secondsText = secondsRemaining < 10 ? $"0{secondsRemaining}" : $"{secondsRemaining}";

        spriteBatch.DrawString(Fonts.Hoog12,
            $"Time Remaining: {minutesRemaining}:{secondsText}",
            new Vector2(0, 0), Color.White);
        base.Draw(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        if (InGameManager.Instance.Paused)
            return;
        timerWait -= gameTime.ElapsedGameTime.Milliseconds;
        if (timerWait <= 0 && InGameManager.Instance.CanMove)
        {
            if (secondsRemaining != 0)
            {
                secondsRemaining--;
            }
            else
            {
                if (minutesRemaining != 0)
                {
                    secondsRemaining = 59;
                    minutesRemaining--;
                }
            }
            timerWait = 1000;
        }

        if (minutesRemaining == 0 && secondsRemaining <= 15) ShowPinch = true;

        if (minutesRemaining == 0 && secondsRemaining == 0 && InGameManager.Instance.CanMove)
        {
            InGameManager.Instance.Winner = true;
            //Call the player death event to end the game.
            EventManager.Instance.GetEvent("playerdeath").Call();
        }

        base.Update(gameTime);
    }
}