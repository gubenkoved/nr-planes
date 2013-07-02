using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    public abstract class Equipment
    {
        // Not marked by [DataMember] to avoid cylce object graph (Plane -> Equipment -> Plane ...)
        public GameObject RelatedGameObject { get; set; }

        /// <summary>
        /// Identificates equipment in related game object's bounds
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public Size Size { get; protected set; }

        /// <summary>
        /// Additional information
        /// </summary>
        [DataMember]
        public string Info { get; set; }        
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