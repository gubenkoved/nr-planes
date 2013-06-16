using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common.Client
{
    /// <summary>
    /// Inherits GamwWorld but brings some client side valuable distinctions.
    /// <para>Client's game world don't delete any game objects by yourself. It wait for signal from server-side game world.</para>
    /// </summary>
    public class ClientGameWorld : GameWorld
    {
        public ClientGameWorld(Size logicalSize)
            : base(logicalSize)
        {

        }

        /// <summary>
        /// There are few differences in client modification for updating game objects:
        /// <para>- Updates existing game objects anyway until it has been removed</para>
        /// <para>- Remove game object only explicitly by server signal (ObjectsSyncronizer instance should to invoke DeleteGameObject method)</para>
        /// </summary>
        /// <param name="elapsed"></param>
        protected override void ProcessGameObjectsUpdate(TimeSpan elapsed)
        {
            using (var handle = m_safeGameObjects.SafeRead())
            {
                List<GameObject> garbage = new List<GameObject>();

                foreach (var gameObject in handle.Items)
                {
                    if (gameObject is Plane)
                    {
                        // updates Plane instances anyway, even when it is become garbage 
                        // to avoid game situations where some plane there is in server's game world and destructed
                        // on the client side - removing Planes only by server signal 
                        // (it organized by ObjectsSyncronizer that uses ExplicitlyRemoveGameObject method)

                        gameObject.Update(elapsed);
                    }
                    else
                    {
                        if (!gameObject.IsGarbage)
                            gameObject.Update(elapsed);
                        else
                            garbage.Add(gameObject);
                    }
                }

                garbage.ForEach(garbageObject => DeleteGameObject(garbageObject));
            }
        }

        public void ExplicitlyRemoveGameObject(GameObject obj)
        {
            base.DeleteGameObject(obj);
        }

        protected override void BeforeDeleteGameObject(GameObject obj)
        {
            // do nothing - bonuses generates only by server
        }

    }
}
