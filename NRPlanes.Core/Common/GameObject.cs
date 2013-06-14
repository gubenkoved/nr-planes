using System;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    [KnownType(typeof(Bullet))]
    [KnownType(typeof(Plane))]
    public abstract class GameObject : IUpdatable, ICloneable
    {
        /// <summary>
        /// Used for unambiguous identification game objects for it parameters updating
        /// Id == null means what object have not committed to server yet
        /// </summary>
        [DataMember]
        public int? Id { get; set; }
        
        /// <summary>
        /// This delegate fills by GameWorld instance, when the object is added to field.
        /// Used for posibility game objects creation by another game objects.
        /// </summary>
        public Action<GameObject> GameWorldAddObjectDelegate;

        /// <summary>
        /// Equals true, when object can be permanently deleted from field
        /// </summary>
        public bool IsGarbage { get; protected set; }

        /// <summary>
        /// Geometry should be specified so that the center of mass point is (0, 0)
        /// </summary>
        [DataMember]
        public Geometry RelativeGeometry { get; protected set; }

        /// <summary>
        /// Returns absolute geomerty. (WARNING: This is VERY expensive operation)
        /// </summary>
        public Geometry CalculateAbsoluteGeometry()
        {
            var tmp = RelativeGeometry.Clone();

            tmp.Rotate(Rotation);
            tmp.Translate(Position);

            return tmp;
        }

        /// <summary>
        /// Rotation angle. From 0 to 360 degres
        /// </summary>
        [DataMember]
        public double Rotation { get; internal set; }

        [DataMember]
        public double RotationVelocity { get; internal set; }

        [DataMember]
        public double RotationAcceleration { get; internal set; }

        [DataMember]
        public Vector Position { get; internal set; }

        [DataMember]
        public Vector Velocity { get; internal set; }

        [DataMember]
        public Vector Acceleration { get; internal set; }

        [DataMember]
        public double Mass { get; private set; }

        [DataMember]
        public double AngularMass { get; private set; }

        [DataMember]
        public ReferenceArea ReferenceArea { get; private set; }

        protected GameObject(double mass, double angularMass, ReferenceArea referenceArea)
        {
            Mass = mass;

            AngularMass = angularMass;

            ReferenceArea = referenceArea;
        }

        public virtual void Update(TimeSpan elapsed)
        {
            //if (!IsGarbage)
                PhysicEngine.UpdateMechanicalParameters(this, elapsed);
        }

        public void Affect(Vector force, Vector fulcrum)
        {
            PhysicEngine.Affect(this, force, fulcrum);
        }

        public void AffectImpulse(Vector impulse, Vector fulcrum)
        {
            PhysicEngine.ImpulseAffect(this, impulse, fulcrum);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}