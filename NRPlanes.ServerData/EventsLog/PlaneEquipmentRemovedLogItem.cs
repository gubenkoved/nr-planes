using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;
using NRPlanes.Core.Common;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class PlaneEquipmentRemovedLogItem : GameEventsLogItem
    {
        [DataMember]
        public readonly int PlaneId;

        [DataMember]
        public readonly int EquipmentId;

        public PlaneEquipmentRemovedLogItem(Timestamp timestamp, Plane plane, PlaneEquipment equipment)
            :base(timestamp)
        {
            PlaneId = plane.Id.Value;
            EquipmentId = equipment.Id;
        }
    }
}
