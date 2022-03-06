using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Tetris.Game;
using Tetris.GUI;

namespace Tetris.Util
{
    public static class Utils
    {
        private static Texture2D sTexture;

        /// <summary>
        ///     This is a more complicated randomizer for Lists, and allows a list to be 'shuffled' around.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                } while (!(box[0] < n * (byte.MaxValue / n)));

                var k = box[0] % n;
                n--;
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        public static IEnumerable<T> SliceRow<T>(this T[,] array, int row)
        {
            for (var i = 0; i < array.GetLength(1); i++)
            {
                yield return array[row, i];
            }
        }
        
        public static void DrawCenteredString(this SpriteBatch spriteBatch, SpriteFont spriteFont,
            string text, Vector2 position, Color color, float shadowOpacity = 0.3f, float opacity = 1f)
        {
            var size = spriteFont.MeasureString(text);
            var pos = position - size / 2f;
            pos.Round();
            spriteBatch.DrawStringWithShadow(spriteFont, text, pos, color * opacity, shadowOpacity);
        }

        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont font, string text,
            Vector2 position, Color color, float shadowOpacity = 0.3f)
        {
            if (Gui.Instance.CurrentScreen == null)
                return;

            spriteBatch.DrawString(font, text, new Vector2(position.X + 3, position.Y + 3),
                Gui.Instance.CurrentScreen.Opacity is > 0.7f and < 1.0f
                    ? Color.Black * (Gui.Instance.CurrentScreen.Opacity - 0.7f)
                    : Gui.Instance.CurrentScreen.Opacity >= 1.0f
                        ? Color.Black * shadowOpacity
                        : Color.Black * 0);
            spriteBatch.DrawString(font, text, position, color * Gui.Instance.CurrentScreen.Opacity);
        }

        /// <summary>
        /// Will create a color gradient(color.lerp) with the given size.
        /// </summary>
        public static Color[] CreateGradient(Color color1, Color color2, int width, int height)
        {
            Color[] pixels = new Color[width*height];
            
            for (int y = 0, w = width, h = height; y < h; y++) {
                var rowOffset = y * w;
                var rowColor = Color.Lerp(color1, color2, (y / (float)h));

                for (int x = 0; x < w; x++)
                    pixels[rowOffset + x] = rowColor;
            }
            return pixels;
        }
        
        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (sTexture == null)
            {
                sTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                sTexture.SetData(new[] {Color.White});
            }

            return sTexture;
        }

        /// <summary>
        ///     Draws a line between two points.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch.</param>
        /// <param name="point1">Point1 (Vector2).</param>
        /// <param name="point2">Point2 (Vector2).</param>
        /// <param name="color">The Color of the Line.</param>
        /// <param name="thickness">The thickness of the line (default is 1f).</param>
        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color,
            float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float) Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawBorderedRect(this SpriteBatch spriteBatch, Rectangle rect, Color inside, Color outside,
            float thickness = 1f)
        {
            spriteBatch.Draw(Globals.TexBox, rect, inside);

            spriteBatch.DrawLine(new Vector2(rect.Left, rect.Top), new Vector2(rect.Left + rect.Width, rect.Top),
                outside, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Left, rect.Top), new Vector2(rect.Left, rect.Bottom), outside,
                thickness);
            spriteBatch.DrawLine(new Vector2(rect.Left, rect.Bottom), new Vector2(rect.Left + rect.Width, rect.Bottom),
                outside, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Right, rect.Top), new Vector2(rect.Left + rect.Width, rect.Bottom),
                outside, thickness);
        }

        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle,
            Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}