using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.GUI.Particle
{
    public class Particle
    {
        protected readonly Random random = new();
        protected float LifeSpan = 15000;

        protected Particle(float x, float y)
        {
        }

        protected float X { get; set; }
        protected float Y { get; set; }
        protected float VelX { get; set; }
        protected float VelY { get; set; }
        protected float Size { get; set; }
        protected float Rotation { get; set; }
        protected Color ParticleColor { get; set; }
        public bool Alive => LifeSpan > 0f && Size > 0f;

        public virtual void Update(GameTime gameTime)
        {
            if (LifeSpan > 0f)
                LifeSpan -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public virtual void DrawParticle(SpriteBatch spriteBatch)
        {
        }

        protected float NextF(float min, float max)
        {
            double range = max - min;
            var sample = random.NextDouble();
            var scaled = sample * range + min;
            var f = (float) scaled;

            return f;
        }
    }
}