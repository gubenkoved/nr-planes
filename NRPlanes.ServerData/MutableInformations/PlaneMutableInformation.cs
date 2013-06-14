using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;
using System.Reflection;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    public class PlaneMutableInformation : GameObjectMutableInformation
    {
        [DataMember]
        public List<PlaneEquipmentMutableInformation> EquipmentMutableInformation;

        [DataMember]
        public double Health;

        private static PrivateValueAccessor m_healthPrivateFieldAccessor = new PrivateValueAccessor(typeof(Plane), "m_health");

        public PlaneMutableInformation(Plane plane)
            :base(plane)
        {
            Health = plane.Health;

            EquipmentMutableInformation = new List<PlaneEquipmentMutableInformation>();

            foreach (var equipment in plane.AllEquipment)
            {
                if (equipment is Engine)
                    EquipmentMutableInformation.Add(new EngineMutableInformation((Engine)equipment));

                if (equipment is Shield)
                    EquipmentMutableInformation.Add(new ShieldMutableInformation((Shield)equipment));
            }
        }

        public override void Apply(object obj)
        {
            base.Apply(obj);

            Plane plane = (Plane)obj;

            m_healthPrivateFieldAccessor.SetValue(obj, Health);

            foreach (var equipmentMutable in EquipmentMutableInformation)
            {
                PlaneEquipment equipment = plane.AllEquipment.Single(e => e.Id == equipmentMutable.Id);

                equipmentMutable.Apply(equipment);
            }
        }

        public override string ToString()
        {
            return string.Format("Health:{0}, GO props:({1})", Health, base.ToString());
        }
    }
}
