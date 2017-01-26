using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Client.Particles
{
    public static class ParticlesEngine
    {
        public static void Update(Particle particle, TimeSpan elapsed)
        {
            if (!particle.IsStatic)
            {
                particle.Position += particle.Velocity * elapsed.TotalSeconds;

                particle.Velocity += particle.Acceleration * elapsed.TotalSeconds;

                particle.Rotation += particle.RotationVelocity * elapsed.TotalSeconds;

                particle.RotationVelocity += particle.RotationAcceleration * elapsed.TotalSeconds;

                particle.SizeFactor += particle.SizeFactorVelocity * elapsed.TotalSeconds;
            }
        }
    }
}
