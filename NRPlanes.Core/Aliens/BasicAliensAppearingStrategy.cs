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
            double x = RandomProvider.NextDouble() * m_world.Size.Width;
            double y = RandomProvider.NextDouble() * m_world.Size.Height;

            Plane alien = XWingPlane.BasicConfiguration(new Vector(x, y));

            base.AddAlienPlaneToField( alien, new AlienPlaneController(m_world, alien));
        }
    }
}
