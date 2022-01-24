using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// File created by Jakob Sailer

// DrawLine by "craftworkgames" https://community.monogame.net/t/line-drawing/6962/4
// DrawPolygon and DrawCircle from this website https://bayinx.wordpress.com/2011/11/07/how-to-draw-lines-circles-and-polygons-using-spritebatch-in-xna/
// it seems this is a method of MonoGame.Extended
// RandomColor by "Idle_Mind" https://stackoverflow.com/questions/32898189/about-calculating-a-random-color

namespace Tetris.Util
{
    internal static class DrawExtension
    {
        private static Texture2D sTexture;

        // getting random colors
        private static readonly Random sRnd = new();

        private static readonly Color[] sColors =
            {Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Purple};

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

        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle,
            Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        ///     Generates a random Color
        /// </summary>
        /// <returns>A random Color.</returns>
        public static Color RandomColor()
        {
            return sColors[sRnd.Next(sColors.Length)];
        }

        /// <summary>
        ///     Draws a polygon from given vertices.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="vertex"></param>
        /// <param name="count"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2[] vertex, int count, Color color,
            float thickness = 1f)
        {
            if (count > 0)
            {
                for (var i = 0; i < count - 1; i++) DrawLine(spriteBatch, vertex[i], vertex[i + 1], color, thickness);

                DrawLine(spriteBatch, vertex[count - 1], vertex[0], color, thickness);
            }
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 center, float size, Color color)
        {
            var origin = new Vector2(0.5f, 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), center, null, color, 0, origin, size, SpriteEffects.None, 0);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, Color color,
            float thickness = 1f, int segments = 16)
        {
            var vertex = new Vector2[segments];

            var increment = Math.PI * 2.0 / segments;
            var theta = 0.0;

            for (var i = 0; i < segments; i++)
            {
                vertex[i] = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                theta += increment;
            }

            DrawPolygon(spriteBatch, vertex, segments, color, thickness);
        }
    }
}