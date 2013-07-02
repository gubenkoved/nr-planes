using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Bullets
{
    [DataContract]
    public class HomingRocket : Bullet
    {
        [DataMember]
        private double m_maxRotationVelocity;

        private Plane m_target;
        public Plane Target 
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
                m_targetPlaneId = m_target.Id.Value;
            }
        }
        
        [DataMember]
        private int m_targetPlaneId;
        public int TargetPlaneId
        {
            get
            {
                return m_targetPlaneId;
            }
        }

        public HomingRocket(Plane target)
            :base(
                20, 
                50,
                PolygonGeometry.FromRectangle( new Rect(Vector.Zero, new Size(1, 3))),
                null,
                200,
                TimeSpan.FromSeconds(30))
        {
            Target = target;

            m_maxRotationVelocity = 10;
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);            

            double angleToTarget = Helper.RelativeAngleBetweenPositions(Position, Target.Position);
            double rotationDelta = Helper.NormalizeAngle(angleToTarget - Rotation);

            // NOTE: it's desirable to change this code from direct manipulation with Rotation prop. (with internal setter)
            // to calculate appretiate force impacting

            if (rotationDelta > 180) // rotate through left side
            {
                Rotation += -1 * m_maxRotationVelocity * elapsed.TotalSeconds;
            }
            else
            {
                Rotation += m_maxRotationVelocity * elapsed.TotalSeconds;
            }
        }
    }
}
