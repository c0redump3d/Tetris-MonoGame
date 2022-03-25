using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris.GUI.Particle;

/// <summary>
///     A boiler-plate class to make adding new particles a very easy process.
/// </summary>
public class Particle
{
    protected readonly Random random = new();

    protected Particle(float x, float y)
    {
    }

    /// <summary>
    ///     Gets the remaining life of the particle(is ms).
    /// </summary>
    public float LifeSpan { get; protected set; } = 5000;

    public float X { get; protected set; }
    public float Y { get; protected set; }
    protected float VelX { get; set; }
    protected float VelY { get; set; }
    public float Size { get; protected set; }
    public float Rotation { get; protected set; }

    protected Color ParticleColor { get; set; }

    //If the particle lifespan is under 0, or size is below 0, the particle is considered dead.
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