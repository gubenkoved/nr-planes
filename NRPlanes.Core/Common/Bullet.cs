﻿using System;
using System.Diagnostics;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    [KnownType(typeof(Bullets.LaserBullet))]
    public abstract class Bullet : GameObject
    {
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

        public static Bullet Prototype()
        {
            throw new InvalidOperationException("Must be hided by derivative class");
        }

        protected Bullet(double mass, double angularMass, ReferenceArea referenceArea, double power, TimeSpan timeToLive)
            : base(mass, angularMass, referenceArea)
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

            IsGarbage = true;
        }
    }
}
