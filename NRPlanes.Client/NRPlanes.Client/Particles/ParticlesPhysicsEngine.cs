using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Client.Particles
{
    public static class ParticlesPhysicsEngine
    {
        public static void Update(Particle particle, TimeSpan elapsed)
        {
            particle.Position += particle.Velocity;

            particle.Velocity += particle.Acceleration;

            particle.Rotation += particle.RotationVelocity;

            particle.RotationVelocity += particle.RotationAcceleration;
        }
    }
}
