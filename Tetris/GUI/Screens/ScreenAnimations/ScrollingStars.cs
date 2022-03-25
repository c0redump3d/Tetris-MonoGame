using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;

namespace Tetris.GUI.Screens.ScreenAnimations;

public class ScrollingStars
{
    private static ScrollingStars _instance;
    private readonly Vector2[] bgPositions = new Vector2[10];

    private readonly Texture2D[] starBackgrounds = new Texture2D[10];

    private ScrollingStars()
    {
        var y = 0;
        //This will essentially create a 5x5 grid of the star picture as the resolution is smaller than the screen rendering size.
        for (var i = 0; i < bgPositions.Length; i++)
        {
            starBackgrounds[i] = Globals.GuiImage[3];
            if (i > 4)
                y = starBackgrounds[0].Height;
            bgPositions[i] = new Vector2(i * starBackgrounds[0].Width, y);
        }
    }

    public static ScrollingStars Instance
    {
        get
        {
            var result = _instance;
            if (result == null) result = _instance ??= new ScrollingStars();

            return result;
        }
    }

    /// <summary>
    ///     Moves the star images across the screen. Also checks to see if any images are currently offscreen.
    /// </summary>
    public void UpdateStarBG()
    {
        for (var i = 0; i < starBackgrounds.Length; i++)
        {
            //If any images are off screen completely, it will find its furthest neighbor on the same y level and move behind that.
            if (bgPositions[i].X > 1280 + starBackgrounds[0].Width)
            {
                float lowestX = 3000;
                foreach (var vec in bgPositions) // find the lowest X position
                    if (vec.X < lowestX && (int) vec.Y == (int) bgPositions[i].Y)
                        lowestX = vec.X;
                bgPositions[i].X = lowestX - starBackgrounds[0].Width;
                break;
            }

            //This controls the speed of scrolling. Found 0.3 to be a fairly nice speed.
            bgPositions[i].X += 0.3f;
        }
    }

    public void DrawStars(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < starBackgrounds.Length; i++)
            spriteBatch.Draw(starBackgrounds[i], bgPositions[i], Color.White);
    }
}