using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;

namespace NRPlanes.Core.Bonuses
{
    public abstract class Bonus : GameObject
    {
        public Bonus()
            :base(0, 0, null)
        {

        }
    }
}
