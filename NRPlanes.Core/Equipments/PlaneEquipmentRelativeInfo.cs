using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    [KnownType(typeof(PlaneWeaponRelativeInfo))]
    public class PlaneEquipmentRelativeInfo
    {
        /// <summary>
        /// Origin of equipment relative to related object origin
        /// </summary>
        [DataMember]
        public Vector RelativeToOriginPosition { get; set; }

        /// <summary>
        /// Related angle of rotation
        /// </summary>
        [DataMember]
        public double RelativeRotation { get; set; }
    }
}
