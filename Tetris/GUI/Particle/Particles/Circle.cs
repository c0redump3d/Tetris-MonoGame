using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;

namespace Tetris.GUI.Particle.Particles;

public class Circle : Particle
{
    private float wait = 10f;

    public Circle(float x, float y) : base(x, y)
    {
        X = x + random.Next(-16, 16);
        Y = y + random.Next(-16, 16);
        Size = GenRandom(1.0F, 2.0F);
    }

    public override void Update(GameTime gameTime)
    {
        wait -= gameTime.ElapsedGameTime.Milliseconds;
        if (Size > 0F)
            Size -= 0.02F;
        if (wait < 0f)
        {
            X += (float) Math.Cos(Size * X);
            Y += (float) Math.Cos(Size * Y);
            wait = 1;
        }

        base.Update(gameTime);
    }

    public override void DrawParticle(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Globals.BallTexture, new Vector2(X, Y), null, Color.White * (Size < 0.5F ? Size : 0.5F), 0.0f,
            Vector2.Zero,
            Size / 45F, SpriteEffects.None, 0f);

        base.DrawParticle(spriteBatch);
    }

    private float GenRandom(float min, float max)
    {
        return NextF(min, max * (max - min + 1.0F));
    }
}