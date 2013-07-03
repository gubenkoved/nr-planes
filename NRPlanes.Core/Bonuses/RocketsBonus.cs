using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Equipments.Weapons;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Core.Bonuses
{
    [DataContract]
    public class RocketsBonus : Bonus
    {
        [DataMember]
        public readonly int Amount;

        public RocketsBonus(Vector position, int amount)
            : base(position)
        {
            Amount = amount;
        }

        protected override void ApplyImpl(Common.Plane plane)
        {
            RocketGun gun = RocketGun.CreateDefault();
            gun.Info = "Homing rockets gun";

            // ToDo: Make flexible mechanism to give the plane possibility manage new equipments relative info
            // so concrete plane instance should decide where new equipment should be placed

            // but now only weird workaround to XWingPlane only
            plane.AddEquipment(gun, new PlaneWeaponRelativeInfo()
            {
                RelativeToOriginPosition = new Vector(0, 6),
                WeaponPosition = WeaponPosition.CenterFront
            });            
        }
    }
}
