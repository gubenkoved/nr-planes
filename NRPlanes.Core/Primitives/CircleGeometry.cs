using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public class CircleGeometry : Geometry
    {
        [DataMember]
        private double m_radius;
        public double Radius
        {
            get
            {
                return m_radius;
            }
        }

        [DataMember]
        private Vector m_center;
        public override Vector Center
        {
            get
            {
                return m_center;
            }
        }

        public override Rect BoundingRectangle
        {
            get { return new Rect(Center.X - Radius, Center.Y - Radius, 2 * Radius, 2 * Radius); }
        }

        public CircleGeometry(Vector center, double radius)
        {
            m_radius = radius;

            m_center = center;
        }

        public override void Translate(Vector translateVector)
        {
            m_center += translateVector;
        }
        public override void Rotate(double rotation)
        {
            m_center = m_center.Rotate(rotation);
        }
        public override bool HitTest(Vector vector)
        {
            return (vector - Center).Length <= Radius;
        }
        public override Geometry Clone()
        {
            return new CircleGeometry(m_center, Radius);
        }
    }
}