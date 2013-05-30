using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    [KnownType(typeof(EngineMutableInformation))]
    [KnownType(typeof(ShieldMutableInformation))]
    public abstract class PlaneEquipmentMutableInformation : MutableInformation
    {
        [DataMember]
        public int Id;

        protected PlaneEquipmentMutableInformation(int id)
        {
            Id = id;
        }
    }
}
