using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public class CircleGeometry : Geometry
    {
        [DataMember]
        private double _radius;
        public double Radius
        {
            get
            {
                return _radius;
            }
        }

        [DataMember]
        private Vector _center;
        public override Vector Center
        {
            get
            {
                return _center;
            }
        }

        public override Rect BoundingRectangle
        {
            get { return new Rect(Center.X - Radius, Center.Y - Radius, 2 * Radius, 2 * Radius); }
        }

        public CircleGeometry(Vector center, double radius)
        {
            _radius = radius;

            _center = center;
        }

        public override void Translate(Vector translateVector)
        {
            _center += translateVector;
        }

        public override void Rotate(double rotation)
        {
            _center = _center.Rotate(rotation);
        }

        public override bool HitTest(Vector vector)
        {
            return (vector - Center).Length <= Radius;
        }

        public override Geometry Clone()
        {
            return new CircleGeometry(_center, Radius);
        }
    }
}