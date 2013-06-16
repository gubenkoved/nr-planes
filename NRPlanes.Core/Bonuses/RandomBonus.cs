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
    public class RandomBonus : Bonus
    {
        [DataMember]
        private Bonus m_underlying;
        public Bonus UnderlyingBonus
        {
            get
            {
                return m_underlying;
            }
        }

        public RandomBonus(Vector position)
            :base(position)
        {
            // ToDo: Instantiation code
        }

        protected override void ApplyImpl(Plane plane)
        {
            m_underlying.Apply(plane);
        }
    }
}
