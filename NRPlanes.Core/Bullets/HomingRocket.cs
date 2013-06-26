using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Bullets
{
    public class HomingRocket : Bullet
    {
        public Plane Target { get; set; }

        public HomingRocket(Plane target)
            :base(
                20, 
                50,
                PolygonGeometry.FromRectangle( new Rect(Vector.Zero, new Size(1, 3))),
                null,
                200,
                TimeSpan.FromSeconds(30))
        {
            Target = target;
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);
        }
    }
}
