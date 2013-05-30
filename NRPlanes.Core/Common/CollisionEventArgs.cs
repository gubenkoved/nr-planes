using System;

namespace NRPlanes.Core.Common
{
    public class CollisionEventArgs : EventArgs
    {
        //public CollisionType Type { get; private set; }

        public Collision Collision { get; private set; }

        public CollisionEventArgs(Collision collision)
        {
            Collision = collision;
        }
    }
}