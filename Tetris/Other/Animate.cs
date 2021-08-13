using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.Other
{
    public static class Animate
    {
        //Animation stuff, needs to be moved to separate class later.
        private static Texture2D _scoreImage = Globals.scoreTextures[0];
        private static float _curOpacity = 0.0F;
        private static bool _animateScore = false;
        private static bool _subOpacity = false;
        private static bool _musicTrans = false;
        private static float _animationSpeed = 0.02F;
        private static int _count = 0;

        private static int Width;
        private static int Height;

        private static float Size;
        //TODO: Get rid of this class entirely and implement a better animation system.
        public static void UpdateAnimation(SpriteBatch _spriteBatch)
        {
            if (!_animateScore) return;

            if (_curOpacity != 1.0F && !_subOpacity)
            {
                _curOpacity += _animationSpeed;
            }

            if (_curOpacity >= 1.0F && !_subOpacity)
            {
                _subOpacity = true;
            }

            if (_subOpacity)
            {
                _curOpacity -= _animationSpeed;
            }

            if (_curOpacity <= 0.0F && _subOpacity)
            {
                if (_musicTrans)
                {
                    Instance.GetSound().PlayMusic(Instance.GetScoreHandler().Level);
                    _musicTrans = false;
                }

                _subOpacity = false;
                _animateScore = false;
                _curOpacity = 0.0F;
                if (Instance.GetGame().GetCountdown())
                {
                    _count++;
                    StartCountdown(_count);
                }
            }
            
            if(Size < 2.0f)
                Size += 0.015f;

            _spriteBatch.Draw(_scoreImage,
                new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) - 5,
                    (_spriteBatch.GraphicsDevice.Viewport.Height / 2)), null,
                Color.White * _curOpacity, 0, new Vector2(Width / 2,Height / 2), Size,
                SpriteEffects.None, 0f);
        }

        public static void StartCountdown(int pos)
        {
            switch (pos)
            {
                case 0:
                    _animationSpeed = 0.03F;
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[0].AnimateImage();
                    break;
                case 1:
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[1].AnimateImage();
                    break;
                case 2:
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[2].AnimateImage();
                    break;
                case 3:
                    _animationSpeed = 0.02F;
                    Globals.countTextures[3].AnimateImage();
                    Instance.GetGame().EndCountdown();
                    Instance.GetGame().StartGame();
                    _count = 0;
                    break;
            }
        }

        /// <summary>
        /// Animates an image at the given location on screen.
        /// </summary>
        /// <param name="image">Image to animate</param>
        /// <param name="point">Location to draw at</param>
        public static void AnimateImage(this Texture2D image)
        {
            _scoreImage = image;
            _curOpacity = 0.0F;
            _animateScore = true;
            Width = image.Width;
            Height = image.Height;
            Size = 0;
            Instance.GetGuiDebug().DebugMessage($"Beginning to animate image: {image.Name}");
        }
        
        public static bool CurrentlyAnimating()
        {
            return _animateScore;
        }
        
        public static void StartLevelWarn()
        {
            Instance.GetSound().StopMusic();
            _musicTrans = true;
            Instance.CurrentLevelUpImage.AnimateImage();
        }
    }
}