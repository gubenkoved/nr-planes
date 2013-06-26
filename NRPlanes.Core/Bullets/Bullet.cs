using System;
using System.Diagnostics;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;
using NRPlanes.Core.Bonuses;
using NRPlanes.Core.Common;

namespace NRPlanes.Core.Bullets
{
    [DataContract]
    [KnownType(typeof(Bullets.LaserBullet))]
    public abstract class Bullet : GameObject
    {
        [DataMember]
        public Guid PlayerGuid { get; set; }

        [DataMember]
        public double Power { get; private set; }

        [DataMember]
        public TimeSpan InitialTimeToLive { get; private set; }

        [DataMember]
        public TimeSpan TimeToLive { get; private set; }

        [DataMember]
        public bool Fired { get; protected set; }

        /// <summary>
        /// If start position is null then bullet is not fired yet
        /// </summary>
        [DataMember]
        public Vector? StartPosition { get; protected set; }

        public static Bullet Default
        {
            get
            {
                throw new InvalidOperationException("Must be hided by derivative class");
            }
        }

        protected Bullet(double mass, double angularMass, Geometry relativeGeometry, ReferenceArea referenceArea, double power, TimeSpan timeToLive)
            : base(mass, angularMass, relativeGeometry, referenceArea)
        {
            Power = power;

            InitialTimeToLive = timeToLive;
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            if (Fired)
            {
                if (TimeToLive == TimeSpan.Zero)
                    IsGarbage = true;

                TimeToLive = TimeToLive - elapsed > TimeSpan.Zero
                                        ? TimeToLive - elapsed
                                        : TimeSpan.Zero;
            }
        }

        public void Fire(Vector initialPosition, Vector initialVelocity, double initialRotation)
        {
            Fired = true;

            TimeToLive = InitialTimeToLive;

            StartPosition = Position = initialPosition;

            Rotation = initialRotation;

            Velocity = initialVelocity;
        }

        public void CollideWith(GameObject obj)
        {
            if (obj is Plane)
            {
                Plane plane = (Plane)obj;
                plane.Damage(Power);
                plane.AffectImpulse(Velocity * Mass, Position);             
            }
            else if (obj is Bullet)
            {
                Bullet bullet = (Bullet)obj;
                bullet.IsGarbage = true;
            }
            else if (obj is Bonus)
            {
                ((Bonus)obj).Damage(Power);
            }

            IsGarbage = true;
        }
    }
}
