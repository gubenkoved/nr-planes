using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;

namespace NRPlanes.Core.Controllers
{
    public abstract class PlaneControllerBase
    {
        public readonly Plane ControlledPlane;

        public PlaneControllerBase(Plane controlledPlane)
        {
            ControlledPlane = controlledPlane;
        }

        /// <summary>
        /// This function shuold be invoked by every game tick.
        /// Contains management imapcts.
        /// </summary>
        public abstract void Update();
    }
}
