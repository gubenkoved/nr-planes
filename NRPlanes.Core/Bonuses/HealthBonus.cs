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
        public readonly double Value;

        public HealthBonus(Vector position, double value)
            : base(position)
        {
            Value = value;
        }

        protected override void ApplyImpl(Plane plane)
        {
            plane.Recover(Value);
        }
    }
}
