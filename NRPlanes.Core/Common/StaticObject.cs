using System;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    [KnownType(typeof(StaticObjects.Planet))]
    [KnownType(typeof(StaticObjects.RectangleGravityField))]
    public abstract class StaticObject
    {
        [DataMember]
        public Geometry AbsoluteGeometry { get; private set; }

        protected StaticObject(Geometry absoluteGeometry)
        {
            AbsoluteGeometry = absoluteGeometry;
        }

        public virtual void AffectOnGameObject(GameObject obj, TimeSpan duration)
        {
            // by default static objects DOES NOT affect the game objects
        }
    }
}