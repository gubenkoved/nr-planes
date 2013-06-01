using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.Particles
{
    public class ParticlesEmitter
    {
        private Random m_random = new Random(Environment.TickCount);

        public readonly GameWorldXna m_world;

        public double LongitualPositionDeviationRadius;
        public double TransversePositionDeviationRadius;

        public double VelocityDeviationRadius;
        public double RotationDeviation;
        public double RotationVelocityDeviation;
        public double AlphaVelocityDeviationFactor;

        public ParticlesEmitter(GameWorldXna world)
        {
            m_world = world;
        }

        public void Emit(Particle particle, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                Particle p = (Particle)particle.Clone();

                DeviateParams(p);

                m_world.AddParticle(p);
            }
            
        }

        private void DeviateParams(Particle particle)
        {
            particle.Position += GenerateAsymmetricRandomVectorWithRadius(particle.Velocity, LongitualPositionDeviationRadius, TransversePositionDeviationRadius);

            particle.Velocity += GenerateRandomVectorWithRadius(VelocityDeviationRadius);
            particle.Rotation += 2 * (m_random.NextDouble() - 0.5) * RotationDeviation;
            particle.RotationVelocity += 2 * (m_random.NextDouble() - 0.5) * RotationVelocityDeviation;
            particle.AlphaVelocity *= (m_random.NextDouble() + 1) * AlphaVelocityDeviationFactor;
        }

        private Vector GenerateAsymmetricRandomVectorWithRadius(Vector ort, double longitualRadius, double transverseRadius)
        {
            Vector v = new Vector((m_random.NextDouble() - 0.5) * 2 * transverseRadius,
                (m_random.NextDouble() - 0.5) * 2 * longitualRadius);

            return v.Rotate(ort.Angle());
        }

        private Vector GenerateRandomVectorWithRadius(double radius)
        {
            return new Vector(
                (m_random.NextDouble() - 0.5) * 2 * radius,
                (m_random.NextDouble() - 0.5) * 2 * radius);

        }
    }
}
