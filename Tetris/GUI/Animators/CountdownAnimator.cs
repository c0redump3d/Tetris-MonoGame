using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.Managers;
using Tetris.Sound;

namespace Tetris.GUI.Animators;

public class CountdownAnimator
{
    private static CountdownAnimator _instance;
    private readonly Texture2D[] countdownImage = Globals.CountTextures;

    public static CountdownAnimator Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new CountdownAnimator();

            return result;
        }
    }

    /// <summary>
    ///     Starts animating the countdown for starting a game/un-pausing game.
    /// </summary>
    public void BeginCountdown()
    {
        //This is the power of the TimerUtil :D(Remembering how poorly this was previously)
        Sfx.Instance.PlaySoundEffect("count");
        Gui.Instance.AnimateImage(countdownImage[0], 1000f, (s, o) =>
        {
            Sfx.Instance.PlaySoundEffect("count");
            Gui.Instance.AnimateImage(countdownImage[1], 1000f, (s2, o2) =>
            {
                Sfx.Instance.PlaySoundEffect("count");
                Gui.Instance.AnimateImage(countdownImage[2], 1000f, (s3, o3) =>
                {
                    EndCountdown();
                    Gui.Instance.AnimateImage(countdownImage[3]);
                });
            });
        });
    }

    /// <summary>
    ///     Ends the countdown (duhhh).
    /// </summary>
    private void EndCountdown()
    {
        InGameManager.Instance.EndCountdown();
        InGameManager.Instance.StartGame();
    }
}