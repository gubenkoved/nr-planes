using System;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace NRPlanes.Core.StaticObjects
{
    [DataContract]
    public class HealthRecoveryPlanet : Planet
    {
        [DataMember]
        public double RecoverySpeed { get; private set; }

        [DataMember]
        private List<GameObject> m_lastTickAffections;
        public IEnumerable<GameObject> AffectedGameObjects
        {
            get
            {
                return m_lastTickAffections;
            }
        }

        public HealthRecoveryPlanet(Vector center, double radius, double recoverySpeed)
            : base(center, radius)
        {
            RecoverySpeed = recoverySpeed;

            m_lastTickAffections = new List<GameObject>();
        }

        public override void Update(TimeSpan elapsed)
        {
            m_lastTickAffections.Clear();
            
            base.Update(elapsed);
        }

        public override void AffectOnGameObject(GameObject obj, TimeSpan duration)
        {
            if (obj is Plane)
            {
                Plane plane = (Plane) obj;

                if (plane.Health != plane.MaximalHealth)
                {
                    m_lastTickAffections.Add(plane);
                    plane.Recover(duration.TotalSeconds * RecoverySpeed);
                }
            }
        }

    }
}