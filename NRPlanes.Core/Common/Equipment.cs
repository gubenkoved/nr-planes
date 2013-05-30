using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    public abstract class Equipment
    {
        // Not marked by [DataMember] to avoid cylce object graph (Plane -> Equipment -> Plane ...)
        public GameObject RelatedGameObject { get; set; }

        /// <summary>
        /// To unambiguous identificate equipment in related game object bounds
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Size Size { get; protected set; }

        /// <summary>
        /// Origin of equipment relative to related object origin
        /// </summary>
        [DataMember]
        public Vector RelativeToOriginPosition { get; protected set; }

        /// <summary>
        /// Related angle of rotation
        /// </summary>
        [DataMember]
        public double RelativeRotation { get; protected set; }

        /// <summary>
        /// Additional information
        /// </summary>
        [DataMember]
        public string Info { get; set; }

        public Vector GetAbsolutePosition()
        {
            return RelatedGameObject.Position + RelativeToOriginPosition.Rotate(RelatedGameObject.Rotation);
        }

        public double GetAbsoluteRotation()
        {
            return RelatedGameObject.Rotation + RelativeRotation;
        }
    }

    [DataContract]
    public abstract class Equipment<T> : Equipment where T : GameObject
    {
        public new T RelatedGameObject
        {
            get { return base.RelatedGameObject as T; }
            set { base.RelatedGameObject = value; }
        }
    }
}