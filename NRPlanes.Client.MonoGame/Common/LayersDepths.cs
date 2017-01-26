using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Client.Common
{
    /// <summary>
    /// Contains different objects layer's depth
    /// </summary>
    public static class LayersDepths
    {
        public const float Background = 1.0f;
        public const float StaticObject = 0.95f;
        public const float GravityField = 0.90f;
        public const float Bonuses = 0.87f;
        public const float ParticlesDefault = 0.85f;        
        public const float Engine = 0.8f;
        public const float Bullet = 0.3f;
        public const float Shield = 0.21f;
        public const float Weapon = 0.2f;
        public const float Plane = 0.1f;
        public const float Explosion = 0.09f;
        public const float BonusesParticles = 0.08f;
    }
}
