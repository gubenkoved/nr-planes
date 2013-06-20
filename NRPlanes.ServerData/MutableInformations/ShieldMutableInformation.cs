using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    public class ShieldMutableInformation : PlaneEquipmentMutableInformation
    {
        [DataMember]
        public bool IsActive;

        public ShieldMutableInformation(Shield shield)
            : base(shield.Id)
        {
            IsActive = shield.IsActive;
        }

        public override void Apply(object obj)
        {
            Shield shield = (Shield)obj;

            if (shield.IsActive && !IsActive)
                shield.TurnOff();

            if (!shield.IsActive && IsActive)
                shield.TurnOn();
        }
    }
}
