using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    public class PlaneWeaponRelativeInfo : PlaneEquipmentRelativeInfo
    {
        [DataMember]
        public WeaponPosition WeaponPosition { get; set; }
    }
}
