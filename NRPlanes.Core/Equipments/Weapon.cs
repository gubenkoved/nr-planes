using System;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments.Weapons;
using NRPlanes.Core.Common;
using NRPlanes.Core.Bullets;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    [KnownType(typeof(LaserGun))]
    [KnownType(typeof(RocketGun))]
    public abstract class Weapon : PlaneEquipment
    {
        protected Weapon(TimeSpan minimalTimeBetweenShots, double initialVelocity, double maximumCharge, double regeneration, Bullet bulletPrototype, Vector bulletOffset)
            : base(maximumCharge, regeneration)
        {
            BulletOffset = bulletOffset;
            BulletPrototype = bulletPrototype;
            InitialBulletVelocity = new Vector(0, initialVelocity);
            ReloadingTime = minimalTimeBetweenShots;
        }

        /// <summary>
        /// Offset between this weapon position and bullet position
        /// </summary>
        [DataMember]
        public Vector BulletOffset { get; private set; }

        [DataMember]
        public Bullet BulletPrototype { get; private set; }

        [DataMember]
        public Vector InitialBulletVelocity { get; private set; }

        /// <summary>
        /// Minimal time between shots
        /// </summary>
        [DataMember]
        public TimeSpan ReloadingTime { get; private set; }

        [DataMember]
        public TimeSpan ElapsedTimeForShot { get; private set; }

        [DataMember]
        public DateTime LastShotDateTime { get; private set; }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            ElapsedTimeForShot = ElapsedTimeForShot - elapsed > TimeSpan.Zero
                                     ? ElapsedTimeForShot - elapsed
                                     : TimeSpan.Zero;
        }

        private void AffectByMomentumConservationLaw(Bullet bullet)
        {
            double absoluteRotation = RelatedGameObject.GetEquipmentAbsoluteRotation(this);

            Vector impulse = (InitialBulletVelocity * bullet.Mass).Rotate(absoluteRotation);

            RelatedGameObject.AffectImpulse(-impulse, bullet.Position);
        }

        public void Fire()
        {
            if (Charge >= 1.0 && ElapsedTimeForShot == TimeSpan.Zero)
            {                
                Charge -= 1.0;
                ElapsedTimeForShot = ReloadingTime;
                LastShotDateTime = DateTime.Now;

                double absoluteRotation = RelatedGameObject.GetEquipmentAbsoluteRotation(this);
                Vector absolutePosition = RelatedGameObject.GetEquipmentAbsolutePosition(this);
                Vector initialVelocity = RelatedGameObject.Velocity + InitialBulletVelocity.Rotate(absoluteRotation);

                Bullet bullet = (Bullet)BulletPrototype.Clone();
                bullet.PlayerGuid = RelatedGameObject.PlayerGuid;
                bullet.Fire(absolutePosition + BulletOffset.Rotate(absoluteRotation), initialVelocity, absoluteRotation);

                AffectByMomentumConservationLaw(bullet);

                RelatedGameObject.GameWorldAddObjectDelegate.Invoke(bullet);
            }
        }
    }
}