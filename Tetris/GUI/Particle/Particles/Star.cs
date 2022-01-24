using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;

namespace Tetris.GUI.Particle.Particles
{
    public class Star : Particle
    {
        private readonly int StarKind;

        public Star(float x, float y) : base(x, y)
        {
            X = x + random.Next(-8, 8);
            Y = y + random.Next(-8, 8);
            StarKind = random.Next(0, 7);
            Size = NextF(1.0F, 3.0F);
        }

        public override void Update(GameTime gameTime)
        {
            if (Size > 0F)
                Size -= 0.05F;
            X += (float) random.NextDouble() * (random.Next(0, 2) == 1 ? 1 : -1);
            Y += (float) random.NextDouble() * (random.Next(0, 2) == 1 ? 1 : -1);
            base.Update(gameTime);
        }

        public override void DrawParticle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Globals.StarTexture[StarKind], new Vector2(X, Y), null, Color.White, 0.0f, Vector2.Zero,
                Size / 4F, SpriteEffects.None, 0f);

            base.DrawParticle(spriteBatch);
        }
    }
}