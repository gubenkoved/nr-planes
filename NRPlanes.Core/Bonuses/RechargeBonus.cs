using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Bonuses
{
    [DataContract]
    public class RechargeBonus : Bonus
    {
        public RechargeBonus(Vector position)
            :base(position)
        {

        }

        protected override void ApplyImpl(Common.Plane plane)
        {
            throw new NotImplementedException();
        }
    }
}
