using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    [KnownType(typeof(GameObjectMutableInformation))]
    [KnownType(typeof(PlaneEquipmentMutableInformation))]
    public abstract class MutableInformation
    {
        public abstract void Apply(object obj);
    }
}
