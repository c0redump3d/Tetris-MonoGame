using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tetris.Multiplayer.Network;

namespace Tetris.GUI.Particle
{
    public class ParticleManager
    {
        private readonly List<Particle> activeParticles = new();

        public void Create(Particle particle)
        {
            activeParticles.Add(particle);
        }

        public void UpdateParticles(GameTime gameTime)
        {
            foreach (var particle in activeParticles.ToList())
            {
                //If the particle is considered 'dead' remove it from particle list.
                if (!particle.Alive) activeParticles.Remove(particle);
                particle.Update(gameTime);
            }
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            //Draws particles relative to the in-game board.
            spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(NetworkManager.Instance.Connected ? 279 : 480, 32, 0)));
            foreach (var particle in activeParticles.ToList()) particle.DrawParticle(spriteBatch);
            spriteBatch.End();
        }

        
        private static ParticleManager _instance;
        public static ParticleManager Instance
        {
            get
            {
                var result = _instance;
                if (result == null)
                {
                    result = _instance ??= new ParticleManager();
                }

                return result;
            }
        }
    }
}