using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.GUI.Animators;

public class ImageAnimator
{
    /// <summary>
    ///     Returns the half point of the provided time for animating(ex: time=1000ms then, halfPoint=500ms)
    /// </summary>
    private readonly float halfPoint;

    /// <summary>
    ///     Returns the current instance's image that is being animated.
    /// </summary>
    private readonly Texture2D image;

    /// <summary>
    ///     Event is raised once the animation has completed on the image.
    /// </summary>
    private readonly EventHandler onFinish;

    /// <summary>
    ///     The frequency in which the animation is updated(16.67ms => 60fps, 8.33ms => 120fps)
    /// </summary>
    private float PropertyTime { get; set; } = 8.33f;

    /// <summary>
    ///     By instantiating and providing a Texture2D, the image will animate on screen.
    /// </summary>
    /// <param name="img">Texture2D you wish to animate.</param>
    /// <param name="time">Animation time in milliseconds.</param>
    public ImageAnimator(Texture2D img, float time = 1000f, EventHandler runFunc = null)
    {
        image = img;
        RemainingTime = time;
        halfPoint = time / 2f;
        onFinish += runFunc;
    }

    /// <summary>
    ///     Gets or sets an offset for the image's y position.
    /// </summary>
    private float OffsetY { get; set; }

    /// <summary>
    ///     Returns the current opacity of the image.
    /// </summary>
    private float Opacity { get; set; }

    /// <summary>
    ///     Returns the current remaining time for the animated image.
    /// </summary>
    private float RemainingTime { get; set; }

    /// <summary>
    ///     Returns the current size of the image.
    /// </summary>
    private float Size { get; set; }

    /// <summary>
    ///     Returns the width of the animated image.
    /// </summary>
    private int Width => image.Width;

    /// <summary>
    ///     Returns the height of the animated image.
    /// </summary>
    private int Height => image.Height;

    public void DrawImage(SpriteBatch spriteBatch)
    {
        var list = Gui.Instance.CurrentScreen.ImageAnim;
        float totalCount = list.Count;
        OffsetY = spriteBatch.GraphicsDevice.Viewport.Height / 2f;

        //If there is more than one animated image, we need to offset its position.
        if (totalCount > 1)
        {
            //Special case if there are only two animated images.
            if ((int) totalCount == 2)
            {
                list[0].OffsetY -= list[0].Height / 2f * list[0].Size + 10;
                list[1].OffsetY += list[1].Height / 2f * list[1].Size + 10;
            }
            else
            {
                //This will set the offset of the image based on its index.
                for (var i = 1; i < totalCount; i++)
                    if (list[i] == this)
                    {
                        //Even indexes are placed above, odd indexes are placed below. 
                        if (i % 2 == 0)
                            OffsetY = list[i - 1].OffsetY - Height * i;
                        else
                            OffsetY = list[i - 1].OffsetY + Height * i;
                    }
            }
        }

        //Finally time to draw the image.
        spriteBatch.Begin();
        spriteBatch.Draw(image,
            new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2 - 5,
                OffsetY), null,
            Color.White * Opacity, 0, new Vector2(Width / 2f, Height / 2f), Size,
            SpriteEffects.None, 0f);
        spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        RemainingTime -= gameTime.ElapsedGameTime.Milliseconds;
        PropertyTime -= gameTime.ElapsedGameTime.Milliseconds;

        //Properties(size, opacity) need to updated at a specified rate.
        if (PropertyTime <= 0)
        {
            //Opacity is increased until it reaches the half point of its animation time, then it is decreased.
            if (RemainingTime >= halfPoint)
            {
                if (Opacity < 1f) Opacity += 0.035f;
            }
            else
            {
                if (Opacity > 0f) Opacity -= 0.035f;
            }

            //Size is always increased until it reaches at or above 2.00.
            if (Size < 2f) Size += 0.025f;

            //Wait 8.33ms until next update.
            PropertyTime = 8.33f;
        }

        //Image animating has completed, now clean up.
        if (RemainingTime <= 0)
        {
            onFinish?.Invoke(this, EventArgs.Empty);
            Gui.Instance.CurrentScreen.ImageAnim.Remove(this);
        }
    }
}