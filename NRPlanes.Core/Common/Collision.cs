using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common
{
    public class Collision
    {
        public readonly GameObject FirstObject;
        public readonly GameObject SecondObject;
        public readonly Vector Position;

        public bool IsSelfCollision
        {
            get
            {
                return SecondObject == null;
            }
        }

        public Collision(GameObject first, GameObject second, Vector collisionPosition)
        {
            FirstObject = first;
            SecondObject = second;
            Position = collisionPosition;
        }

        /// <summary>
        /// Self collision occurs by server signal (e.g. planes destruction)
        /// </summary>
        public static Collision CreateSelfCollision(GameObject obj)
        {
            return new Collision(obj, null, obj.Position);
        }
    }
}