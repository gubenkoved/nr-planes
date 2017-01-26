using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;

namespace NRPlanes.Client.Particles
{
    public class SymmetricParticlesEmitter : ParticlesEmitterBase
    {        
        public double PositionDeviationRadius;
        public double VelocityDeviationRadius;

        /// <summary>
        /// Deviates rotation property with +-RotationDeviation degrees
        /// </summary>
        public double RotationDeviation;

        /// <summary>
        /// Deviates rotation velocity property with +-RotationDeviation degrees/sec
        /// </summary>
        public double RotationVelocityDeviation;

        public double AlphaVelocityDeviationFactor;

        public SymmetricParticlesEmitter(GameWorldXna world)
            :base(world)
        {

        }

        protected override void HandleCreatedParticle(Particle particle)
        {
            DeviateParticleParams(particle);
        }

        private void DeviateParticleParams(Particle particle)
        {
            particle.Position += GenerateRandomVectorWithRadius(PositionDeviationRadius);
            particle.Velocity += GenerateRandomVectorWithRadius(VelocityDeviationRadius);
            particle.Rotation += (RandomProvider.NextDouble() - 0.5) * 2 * RotationDeviation;
            particle.RotationVelocity += (RandomProvider.NextDouble() - 0.5) * 2 * RotationVelocityDeviation;

            particle.AlphaVelocity *= 1 + 2 * (RandomProvider.NextDouble() - 0.5) * AlphaVelocityDeviationFactor;
        }

        private Vector GenerateRandomVectorWithRadius(double radius)
        {
            double angle = RandomProvider.NextDouble() * 360;

            return new Vector(0, (RandomProvider.NextDouble() - 0.5) * 2 * radius).Rotate(angle);
        }
    }
}
