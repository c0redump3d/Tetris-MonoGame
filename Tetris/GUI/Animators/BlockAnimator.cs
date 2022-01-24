using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Game.InGame;
using Tetris.GUI.Particle;
using Tetris.GUI.Particle.Particles;
using Tetris.Util;

namespace Tetris.GUI.Animators
{
    public class BlockAnimator
    {
        private int bubbleTime = 100;
        public Rectangle[] CopyCat = new Rectangle[10];
        public Texture2D[] CopyCatTex = new Texture2D[10];
        public Rectangle[] Laser = new Rectangle[2];
        public Color LaserColor;
        public float Opacity = 1.0f;
        private readonly int StartY;

        public BlockAnimator(int y)
        {
            StartY = y;
            Laser[0] = new Rectangle(160, StartY + 8, 0, 12);
            Laser[1] = new Rectangle(160, StartY + 8, 0, 12);
            for (var i = 0; i < 10; i++)
            for (var f = 0; f < TetrisBoard.Instance.Board.GetLength(0); f++)
                if (f * 32 + Globals.LowestY == StartY)
                {
                    CopyCat[i] = new Rectangle(i * 32, f * 32 + Globals.LowestY, 32, 32);
                    CopyCatTex[i] = Globals.BlockTexture[TetrisBoard.Instance.Board[f, i] - 1];
                }

            LaserColor = DrawExtension.RandomColor();
            Opacity = 1.0f;
        }

        public void Update(GameTime gameTime)
        {
            bubbleTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (bubbleTime < 0)
            {
                ParticleManager.Instance.Create(new Star(Laser[0].X, Laser[0].Y));
                ParticleManager.Instance.Create(new Star(Laser[1].X + Laser[1].Width, Laser[1].Y));
                bubbleTime = 100;
            }

            if (Laser[0].X > 0)
            {
                Laser[0].X -= 5;
                Laser[0].Width += 5;
            }

            if (Laser[1].Width < 160) Laser[1].Width += 5;

            if (Opacity > 0.0f)
                Opacity -= 0.02f;
        }
    }
}