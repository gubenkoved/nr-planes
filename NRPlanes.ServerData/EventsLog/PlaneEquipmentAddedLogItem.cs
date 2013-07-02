using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;
using NRPlanes.Core.Equipments;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class PlaneEquipmentAddedLogItem : GameEventsLogItem
    {
        [DataMember]
        public readonly int PlaneId;

        [DataMember]
        public readonly PlaneEquipment Equipment;

        [DataMember]
        public readonly PlaneEquipmentRelativeInfo EquipmentRelativeInfo;

        public PlaneEquipmentAddedLogItem(Timestamp timestamp, Plane plane, PlaneEquipment equipment)
            :base(timestamp)
        {
            PlaneId = plane.Id.Value;
            Equipment = equipment;
            EquipmentRelativeInfo = plane.GetEquipmentRelativeInfo(equipment);
        }
    }
}
