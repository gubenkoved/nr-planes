using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Bonuses
{
    [DataContract]
    public class HealthBonus : Bonus
    {
        [DataMember]
        private double m_value;
        public double Value
        {
            get
            {
                return m_value;
            }
        }

        public HealthBonus(Vector position, double value)
            : base(position)
        {
            m_value = value;
        }

        protected override void ApplyImpl(Plane plane)
        {
            plane.Recover(m_value);
        }
    }
}
