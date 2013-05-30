using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Controllers;
using NRPlanes.Core.Logging;

namespace NRPlanes.Core.Aliens
{
    public class BasicAliensAppearingStrategy : AliensAppearingStrategy
    {
        private static Random m_random = new Random(Environment.TickCount);

        private TimeSpan m_timeToAppearing;
        private TimeSpan m_appearingPeriod;

        public BasicAliensAppearingStrategy(GameWorld world, TimeSpan appearingPeriod)
            : base(world)
        {
            m_timeToAppearing = appearingPeriod;
            m_appearingPeriod = appearingPeriod;
        }

        public override void Update(TimeSpan elapsed)
        {
            m_timeToAppearing -= elapsed;

            if (m_timeToAppearing.TotalSeconds <= 0)
            {
                AddAlienToGameField();

                m_timeToAppearing = m_appearingPeriod;
            }
        }

        private void AddAlienToGameField()
        {
            Logger.Log("Add alien's plane...");

            double x = m_random.NextDouble() * m_world.Size.Width;
            double y = m_random.NextDouble() * m_world.Size.Height;

            Plane alien = XWingPlane.BasicConfiguration(new Vector(x, y));

            m_world.AddGameObject(alien);
            m_world.AddPlaneController(new AlienPlaneController(m_world, alien));            
        }
    }
}
