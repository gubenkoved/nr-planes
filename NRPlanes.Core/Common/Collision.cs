using NRPlanes.Core.Primitives;
using System;

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

        /// <summary>
        /// Returns true when collided object has specified types
        /// </summary>
        public bool CheckTypes(Type type1, Type type2)
        {
            return type1.IsInstanceOfType(FirstObject) && type2.IsInstanceOfType(SecondObject)
                || type2.IsInstanceOfType(FirstObject) && type1.IsInstanceOfType(SecondObject);
        }
        /// <summary>
        /// Returns true when both collided object has specified types
        /// </summary>
        public bool CheckTypes(Type type)
        {
            return type.IsInstanceOfType(FirstObject) && type.IsInstanceOfType(SecondObject);                
        }
    }
}