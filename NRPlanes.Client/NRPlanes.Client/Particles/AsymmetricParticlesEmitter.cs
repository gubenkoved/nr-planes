using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.Particles
{
    public class AsymmetricParticlesEmitter : ParticlesEmitterBase
    {
        private Random m_random = new Random(Environment.TickCount);

        public double LongitualPositionDeviationRadius;
        public double TransversePositionDeviationRadius;

        public double LongitualVelocityDeviationRadius;
        public double TransverseVelocityDeviationRadius;

        public double RotationDeviation;
        public double RotationVelocityDeviation;

        /// <summary>
        /// Correct values: [0; 1)
        /// </summary>
        public double AlphaVelocityDeviationFactor;

        public AsymmetricParticlesEmitter(GameWorldXna world)
            :base( world )
        {
        }

        protected override void HandleCreatedParticle(Particle particle)
        {
            DeviateParams(particle);
        }

        private void DeviateParams(Particle particle)
        {
            particle.Position += GenerateAsymmetricRandomVectorWithRadius(particle.Velocity, LongitualPositionDeviationRadius, TransversePositionDeviationRadius);
            particle.Velocity += GenerateAsymmetricRandomVectorWithRadius(particle.Velocity, LongitualVelocityDeviationRadius, TransverseVelocityDeviationRadius);

            particle.Rotation += 2 * (m_random.NextDouble() - 0.5) * RotationDeviation;
            particle.RotationVelocity += 2 * (m_random.NextDouble() - 0.5) * RotationVelocityDeviation;

            particle.AlphaVelocity *= 1 + 2 * (m_random.NextDouble() - 0.5) * AlphaVelocityDeviationFactor;
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
