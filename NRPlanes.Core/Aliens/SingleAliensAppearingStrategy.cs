using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Logging;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Controllers;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Aliens
{
    public class SingleAliensAppearingStrategy : AliensAppearingStrategy
    {
        private bool m_added = false;
        private static Random m_random = new Random(Environment.TickCount);

        public SingleAliensAppearingStrategy(GameWorld world)
            :base(world)
        {

        }

        public override void Update(TimeSpan elapsed)
        {
            if (!m_added)
            {
                Logger.Log("Add alien's plane...");

                double x = m_random.NextDouble() * m_world.Size.Width;
                double y = m_random.NextDouble() * m_world.Size.Height;

                Plane alien = XWingPlane.BasicConfiguration(new Vector(x, y));

                m_world.AddGameObject(alien);
                m_world.AddPlaneController(new AlienPlaneController(m_world, alien));

                m_added = true;
            }
        }
    }
}
