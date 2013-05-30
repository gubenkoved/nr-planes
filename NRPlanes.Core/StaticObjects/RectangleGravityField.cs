using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.StaticObjects
{
    [DataContract]
    public class RectangleGravityField : StaticObject
    {
        [DataMember]
        public Vector ForceDirection { get; private set; }

        [DataMember]
        public double Acceleration { get; private set; }

        public RectangleGravityField(Rect rectangle, Vector forceDirection, double acceleration)
            : base(PolygonGeometry.FromRectangle(rectangle))
        {
            ForceDirection = forceDirection.Ort();

            Acceleration = acceleration;
        }

        public override void AffectOnGameObject(GameObject obj, System.TimeSpan duration)
        {
            obj.Affect(obj.Mass * Acceleration * ForceDirection, obj.Position);
        }
    }
}