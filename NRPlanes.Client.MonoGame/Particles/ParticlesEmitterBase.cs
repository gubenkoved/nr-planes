using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;

namespace NRPlanes.Client.Particles
{
    public abstract class ParticlesEmitterBase
    {
        private readonly GameWorldXna m_world;

        public ParticlesEmitterBase(GameWorldXna world)
        {
            m_world = world;
        }

        public void Emit(Particle particle, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                Particle p = (Particle)particle.Clone();

                HandleCreatedParticle(p);

                m_world.AddParticle(p);
            }

        }

        protected virtual void HandleCreatedParticle(Particle particle)
        {
        }
    }
}
