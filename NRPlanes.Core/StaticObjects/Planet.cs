using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.StaticObjects
{
    [DataContract]
    [KnownType(typeof(HealthRecoveryPlanet))]
    public abstract class Planet : StaticObject
    {
        protected Planet(Vector center, double radius)
            : base(new CircleGeometry(center, radius))
        {
        }
    }
}