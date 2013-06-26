using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Core.Common
{
    public class ExplosionEventArgs : EventArgs
    {
        public readonly GameObject Exploded;

        public ExplosionEventArgs(GameObject exploded)
        {
            Exploded = exploded;
        }
    }
}
