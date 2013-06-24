using System;

namespace NRPlanes.Core.Common
{
    public class CollisionEventArgs : EventArgs
    {
        public readonly Collision Collision;

        public CollisionEventArgs(Collision collision)
        {
            Collision = collision;
        }
    }
}