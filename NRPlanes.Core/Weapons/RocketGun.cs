using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Core.Weapons
{
    [DataContract]
    public class RocketGun : Weapon
    {
        public RocketGun(TimeSpan minimalTimeBetweenShots, double initialVelocity, double maximumCharge, double regeneration, Bullet bulletPrototype, Vector bulletOffset)
            : base(minimalTimeBetweenShots, initialVelocity, maximumCharge, regeneration, bulletPrototype, bulletOffset)
        {
            Size = new Size(2, 2);
        }
    }
}
