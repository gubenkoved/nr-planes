using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common
{
    public class Collision
    {
        public readonly GameObject FirstObject;
        public readonly GameObject SecondObject;
        public readonly Vector Position;

        public Collision(GameObject first, GameObject second, Vector collisionPosition)
        {
            FirstObject = first;
            SecondObject = second;
            Position = collisionPosition;
        }
    }
}