using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Logging;
using NRPlanes.Core.Controllers;

namespace NRPlanes.Core.Aliens
{
    public abstract class AliensAppearingStrategy : IUpdatable
    {
        protected GameWorld m_world;

        public AliensAppearingStrategy(GameWorld world)
        {
            m_world = world;
        }

        public abstract void Update(TimeSpan elapsed);

        protected void AddAlienPlaneToField(Plane plane, AlienPlaneController controller)
        {
            Logger.Log("Add alien's plane...");

            plane.PlayerGuid = new Guid();

            m_world.AddGameObject(plane);
            m_world.AddPlaneController(new AlienPlaneController(m_world, plane));
        }        
    }
}
