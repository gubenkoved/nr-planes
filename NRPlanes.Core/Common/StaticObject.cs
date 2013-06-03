using System;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    /// <summary>
    /// This type represents static game objects.
    /// <para>Static objects does not take part in collision detection</para>
    /// <para>Static objects have not mechanicals params (such as velocity, mass, etc...) - it is static</para>
    /// </summary>
    [DataContract]
    [KnownType(typeof(StaticObjects.Planet))]
    [KnownType(typeof(StaticObjects.RectangleGravityField))]    
    public abstract class StaticObject : IUpdatable
    {
        [DataMember]
        public Geometry AbsoluteGeometry { get; private set; }

        protected StaticObject(Geometry absoluteGeometry)
        {
            AbsoluteGeometry = absoluteGeometry;
        }

        /// <summary>
        /// This function called by physic engine when some game object in specified absolute geometry
        /// </summary>
        public virtual void AffectOnGameObject(GameObject obj, TimeSpan duration)
        {
            // by default static objects DOES NOT affect the game objects
        }

        public virtual void Update(TimeSpan elapsed)
        {
            // do nothing by default
        }
    }
}