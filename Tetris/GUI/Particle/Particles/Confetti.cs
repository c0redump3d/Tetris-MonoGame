using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Game;
using Tetris.Util;

namespace Tetris.GUI.Particle.Particles;

public class Confetti : Particle
{
    private readonly int flyDir;

    public Confetti(float x, float y) : base(x, y)
    {
        X = x + random.Next(-4, 4);
        Y = y;
        Rotation = random.Next(0, 360);
        ParticleColor = DrawExtension.RandomColor();
        VelX = NextF(0.1f, 2f);
        VelY = NextF(1.1f, 8f);
        flyDir = random.Next(0, 2) == 1 ? 1 : -1;
        Size = NextF(1.0F, 15F);
    }

    public override void Update(GameTime gameTime)
    {
        if (VelX > 0f)
        {
            X += VelX * flyDir;
            VelX -= 0.005f;
        }

        if (VelY > -500f)
        {
            Y -= VelY;
            VelY -= 0.05f;
        }

        base.Update(gameTime);
    }

    public override void DrawParticle(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Globals.TexBox, new Vector2(X, Y), null, ParticleColor, Rotation, Vector2.Zero, Size,
            SpriteEffects.None, 0f);

        base.DrawParticle(spriteBatch);
    }
}