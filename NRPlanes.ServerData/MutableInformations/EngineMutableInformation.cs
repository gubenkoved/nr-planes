using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;
using NRPlanes.Core.Equipments;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    public class EngineMutableInformation : PlaneEquipmentMutableInformation
    {
        [DataMember]
        public bool IsActive;

        public EngineMutableInformation(Engine engine)
            :base(engine.Id)
        {
            IsActive = engine.IsActive;
        }

        public override void Apply(object obj)
        {
            Engine engine = (Engine)obj;

            if (engine.IsActive && !IsActive)
                engine.TurnOff();

            if (!engine.IsActive && IsActive)
                engine.TurnOn();
        }
    }
}
