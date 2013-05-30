using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Weapons
{
    [DataContract]
    public class LaserGun : Weapon
    {
        public LaserGun(Vector relativePosition, double relativeRotation, WeaponPosition position, TimeSpan minimalTimeBetweenShots, double initialVelocity, double maximumCharge, double regeneration, Bullet bulletPrototype, Vector bulletOffset)
            : base(position, minimalTimeBetweenShots, initialVelocity, maximumCharge, regeneration, bulletPrototype, bulletOffset)
        {
            Size = new Size(1.2, 1.3);

            RelativeToOriginPosition = relativePosition;

            RelativeRotation = relativeRotation;
        }
    }
}