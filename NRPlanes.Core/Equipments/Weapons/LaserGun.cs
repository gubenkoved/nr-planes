using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Core.Equipments.Weapons
{
    [DataContract]
    public class LaserGun : Weapon
    {
        public static LaserGun CreateDefault()
        {
            return new LaserGun(                
                TimeSpan.FromMilliseconds(120),
                100.0,
                30,
                1.0,
                LaserBullet.Default,
                new Vector(0.0, 1.5));
        }
        

        private LaserGun(TimeSpan minimalTimeBetweenShots, double initialVelocity, double maximumCharge, double regeneration, LaserBullet bulletPrototype, Vector bulletOffset)
            : base(minimalTimeBetweenShots, initialVelocity, maximumCharge, regeneration, bulletPrototype, bulletOffset)
        {
            Size = new Size(1.2, 1.3);
        }
    }
}