using System;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.StaticObjects
{
    [DataContract]
    public class HealthRecoveryPlanet : Planet
    {
        [DataMember]
        public double RecoverySpeed { get; private set; }

        public HealthRecoveryPlanet(Vector center, double radius, double recoverySpeed)
            : base(center, radius)
        {
            RecoverySpeed = recoverySpeed;
        }

        public override void AffectOnGameObject(GameObject obj, TimeSpan duration)
        {
            if (obj is Plane)
            {
                var plane = (Plane) obj;

                plane.Recover(duration.TotalSeconds * RecoverySpeed);
            }
        }

    }
}