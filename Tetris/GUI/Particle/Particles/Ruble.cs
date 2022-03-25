using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;

namespace Tetris.GUI.Particle.Particles;

public class Ruble : Particle
{
    private readonly int FlyDir;
    private readonly int Kind;

    public Ruble(float x, float y) : base(x, y)
    {
        X = x + random.Next(-16, 16);
        Y = y + random.Next(-16, 16);
        VelX = (float) random.NextDouble();
        VelY = (float) random.NextDouble() * 1.75F;
        Kind = random.Next(0, 3);
        FlyDir = random.Next(0, 2) == 1 ? 1 : -1;
        Size = 1;
    }

    public override void Update(GameTime gameTime)
    {
        if (VelX > 0f)
        {
            X += VelX * FlyDir;
            VelX -= 0.005f;
        }

        if (VelY > -100f)
        {
            Y -= VelY;
            VelY -= 0.05f;
        }

        base.Update(gameTime);
    }

    public override void DrawParticle(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Globals.RubleTexture[Kind], new Vector2(X, Y), null, Color.White, Rotation, Vector2.Zero,
            0.3F, SpriteEffects.None, 0f);

        base.DrawParticle(spriteBatch);
    }
}