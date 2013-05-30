using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;

namespace NRPlanes.Core.Controllers
{
    public class AIPlaneController : PlaneControllerBase
    {
        public AIPlaneController(Plane controlled)
            : base(controlled)
        {

        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
