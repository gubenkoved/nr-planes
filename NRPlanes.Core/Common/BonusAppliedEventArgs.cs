using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Bonuses;

namespace NRPlanes.Core.Common
{
    public class BonusAppliedEventArgs : EventArgs
    {
        public readonly Bonus Bonus;
        public readonly Plane Plane;

        public BonusAppliedEventArgs(Bonus bonus, Plane plane)
        {
            Bonus = bonus;
            Plane = plane;
        }

    }
}
