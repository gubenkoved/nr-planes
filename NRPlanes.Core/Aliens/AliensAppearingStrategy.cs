using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;

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

        protected void AddAlienPlaneToField(Plane plane)
        {
            m_world.AddGameObject(plane);
        }
    }
}
