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
        private static Vector2 _scorePoint = new(70, 250);
        private static bool _animateScore = false;
        private static bool _subOpacity = false;
        private static bool _musicTrans = false;
        private static float _animationSpeed = 0.02F;
        private static int _count = 0;
        //TODO: Get rid of this class entirely and implement a better animation system.
        public static void UpdateAnimation(SpriteBatch _spriteBatch)
        {
            if (!_animateScore) return;
            
            if (_curOpacity != 1.0F && !_subOpacity)
            {
                _curOpacity += _animationSpeed;
            }

            if(_curOpacity >= 1.0F && !_subOpacity)
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
                
            _spriteBatch.Draw(_scoreImage, _scorePoint, Color.White * _curOpacity);
        }
        
        public static void StartCountdown(int pos)
        {
            switch (pos)
            {
                case 0:
                    _animationSpeed = 0.03F;
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[0].AnimateImage(new Vector2(120, 300));
                    break;
                case 1:
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[1].AnimateImage(new Vector2(120, 300));
                    break;
                case 2:
                    Instance.GetSound().PlaySoundEffect("count");
                    Globals.countTextures[2].AnimateImage(new Vector2(135, 300));
                    break;
                case 3:
                    _animationSpeed = 0.02F;
                    Globals.countTextures[3].AnimateImage(new Vector2(40, 300));
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
        public static void AnimateImage(this Texture2D image, Vector2 point)
        {
            _scoreImage = image;
            _scorePoint = point;
            _curOpacity = 0.0F;
            _animateScore = true;
        }
        
        public static bool CurrentlyAnimating()
        {
            return _animateScore;
        }
        
        public static void StartLevelWarn()
        {
            Instance.GetSound().StopMusic();
            _musicTrans = true;
            Instance.CurrentLevelUpImage.AnimateImage(new Vector2(65, 250));
        }
    }
}