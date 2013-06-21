using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;

namespace NRPlanes.Client.Particles
{
    public class AsymmetricParticlesEmitter : ParticlesEmitterBase
    {
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

        /// <summary>
        /// Assign longitual direction
        /// </summary>
        public Vector LongitualDirection = new Vector(0, 1);

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
            particle.Position += GenerateAsymmetricRandomVectorWithRadius(LongitualDirection, LongitualPositionDeviationRadius, TransversePositionDeviationRadius);
            particle.Velocity += GenerateAsymmetricRandomVectorWithRadius(LongitualDirection, LongitualVelocityDeviationRadius, TransverseVelocityDeviationRadius);

            particle.Rotation += 2 * (RandomProvider.NextDouble() - 0.5) * RotationDeviation;
            particle.RotationVelocity += 2 * (RandomProvider.NextDouble() - 0.5) * RotationVelocityDeviation;

            particle.AlphaVelocity *= 1 + 2 * (RandomProvider.NextDouble() - 0.5) * AlphaVelocityDeviationFactor;
        }

        private Vector GenerateAsymmetricRandomVectorWithRadius(Vector ort, double longitualRadius, double transverseRadius)
        {
            Vector v = new Vector((RandomProvider.NextDouble() - 0.5) * 2 * transverseRadius,
                (RandomProvider.NextDouble() - 0.5) * 2 * longitualRadius);

            return v.Rotate(ort.Angle());
        }

        private Vector GenerateRandomVectorWithRadius(double radius)
        {
            return new Vector(
                (RandomProvider.NextDouble() - 0.5) * 2 * radius,
                (RandomProvider.NextDouble() - 0.5) * 2 * radius);

        }
    }
}
