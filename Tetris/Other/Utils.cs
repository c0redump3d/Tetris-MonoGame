using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Other
{
    public static class Utils
    {
        private static Texture2D sTexture;

        /// <summary>
        /// This is a more complicated randomizer for Lists, and allows a list to be 'shuffled' around.
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        public static void DrawCenteredString(this SpriteBatch spriteBatch, SpriteFont spriteFont,
            string text, Vector2 position, Color color, float shadowOpacity = 0.3f)
        {
            Vector2 size = spriteFont.MeasureString(text);
            spriteBatch.DrawStringWithShadow(spriteFont, text, position - size / 2f, color, shadowOpacity);
        }
        
        /// <summary>
        /// Used to quickly add a rectangle to the PlacedRect array.
        /// </summary>
        /// <param name="rect">PlacedRect</param>
        /// <param name="newRect">The rectangle you want to add</param>
        public static void Add(this Rectangle[] rect, Rectangle newRect, Texture2D rectTex)
        {
            List<Rectangle> createBlock = rect.ToList();
            createBlock.Add(newRect);
            Instance.GetPlayer().PlacedRect = createBlock.ToArray();
            List<Texture2D> addTex = Instance.GetPlayer().StoredImage.ToList();
            addTex.Add(rectTex);
            Instance.GetPlayer().StoredImage = addTex.ToArray();
        }

        /// <summary>
        /// Used to quickly add a rectangle to the PlacedRect array.
        /// </summary>
        /// <param name="rect">PlacedRect</param>
        /// <param name="index">The index in which you would like to remove</param>
        public static void RemoveAt(this Rectangle[] rect, int index)
        {
            List<Rectangle> removeBlock = rect.ToList();
            List<Texture2D> removeTex = Instance.GetPlayer().StoredImage.ToList();
            removeBlock.RemoveAt(index);
            removeTex.RemoveAt(index);
            Instance.GetPlayer().PlacedRect = removeBlock.ToArray();
            Instance.GetPlayer().StoredImage = removeTex.ToArray();
        }

        /// <summary>
        /// Will give the current shapes Tetris block image.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="placed"></param>
        /// <returns>Tetris Block Image</returns>
        public static Texture2D TranslateShapeToImage(int shape, bool placed)
        {
            if (shape is < 8 and > 0)
            {
                if (placed)
                    return Globals.BlockPlacedTexture[shape - 1];
                
                return Globals.BlockTexture[shape - 1];
            }

            //if something actually important is being drawn to the screen ig it will be the random block image.
            //but, nothing important should ever fall down here.
            return Globals.BlockPlacedTexture[7];
        }

        public static void DrawStringWithShadow(this SpriteBatch spriteBatch, SpriteFont font, string text,
            Vector2 position, Color color, float shadowOpacity = 0.3f)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X + 3, position.Y + 3),
                Instance.GetGui().FadeOpacity is > 0.7f and < 1.0f
                    ?
                    Color.Black * (Instance.GetGui().FadeOpacity - 0.7f)
                    : Instance.GetGui().FadeOpacity >= 1.0f
                        ? Color.Black * shadowOpacity
                        : Color.Black * 0);
            spriteBatch.DrawString(font, text, position, color * Instance.GetGui().FadeOpacity);
        }

        private static Texture2D GetTexture(SpriteBatch spriteBatch) {
            if (sTexture == null) {
                sTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                sTexture.SetData(new[] {Color.White});
            }

            return sTexture;
        }
        
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch.</param>
        /// <param name="point1">Point1 (Vector2).</param>
        /// <param name="point2">Point2 (Vector2).</param>
        /// <param name="color">The Color of the Line.</param>
        /// <param name="thickness">The thickness of the line (default is 1f).</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f) {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float) Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawBorderedRect(this SpriteBatch spriteBatch, Rectangle rect, Color inside, Color outside, float thickness = 1f)
        {
            spriteBatch.Draw(Instance.DebugBox,rect, inside);
            
            spriteBatch.DrawLine(new Vector2(rect.Left,rect.Top), new Vector2(rect.Left + rect.Width,rect.Top), outside, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Left,rect.Top), new Vector2(rect.Left,rect.Bottom), outside, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Left,rect.Bottom), new Vector2(rect.Left + rect.Width,rect.Bottom), outside, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Right,rect.Top), new Vector2(rect.Left + rect.Width,rect.Bottom), outside, thickness);
        }
        
        private static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f) {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
        
        
        /// <summary>
        /// This method is used to translate the current color of the players block color. This is because the network packet sends the color of shape with one letter, for example blue = b.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Color's first letter(ex: Blue = b)</returns>
        public static char TranslateToShapeChar(string block)
        {

            //Could not use switch case here, c# requires that the value be a constant.
            if (block.Contains("_t_"))
            {
                return 'p';
            }
            if (block.Contains("_z1_"))
            {
                return 'r';
            }
            if (block.Contains("_l1_"))
            {
                return 'b';
            }
            if (block.Contains("_i_"))
            {
                return 'c';
            }
            if (block.Contains("_o_"))
            {
                return 'y';
            }
            if (block.Contains("_l2_"))
            {
                return 'g';
            }
            if (block.Contains("_z2_"))
            {
                return 'l';
            }
            if (block.Contains("_x_"))
            {
                return 'x';
            }

            //if something actually important is being drawn to the screen ig it will be the random block image.
            //but, nothing important should ever fall down here.
            return 'x';
        }
        
        /// <summary>
        /// This method is used to translate the current color of the players block color. This is because the network packet sends the color of shape with one letter, for example blue = b.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Color's first letter(ex: Blue = b)</returns>
        public static Texture2D TranslateNameToTexture(string block)
        {

            //Could not use switch case here, c# requires that the value be a constant.
            if (block.Equals("p"))
            {
                return Globals.BlockPlacedTexture[0];
            }
            if (block.Equals("r"))
            {
                return Globals.BlockPlacedTexture[1];
            }
            if (block.Equals("b"))
            {
                return Globals.BlockPlacedTexture[2];
            }
            if (block.Equals("c"))
            {
                return Globals.BlockPlacedTexture[3];
            }
            if (block.Equals("y"))
            {
                return Globals.BlockPlacedTexture[4];
            }
            if (block.Equals("g"))
            {
                return Globals.BlockPlacedTexture[5];
            }
            if (block.Equals("l"))
            {
                return Globals.BlockPlacedTexture[6];
            }
            if (block.Equals("x"))
            {
                return Globals.BlockPlacedTexture[7];
            }

            //if something actually important is being drawn to the screen ig it will be the random block image.
            //but, nothing important should ever fall down here.
            return Globals.BlockPlacedTexture[7];
        }
        
    }
}
