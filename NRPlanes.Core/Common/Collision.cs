using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common
{
    public class Collision
    {
        public GameObject One { get; private set; }

        public GameObject Two { get; private set; }

        public Vector CollisionPosition { get; private set; }

        public Collision(GameObject one, GameObject two, Vector collisionPosition)
        {
            One = one;

            Two = two;

            CollisionPosition = collisionPosition;
        }
    }
}