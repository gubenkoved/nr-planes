using System;

namespace NRPlanes.Core.Common
{
    public enum GameObjectStatus
    {
        Created,
        Deleted
    }

    public class GameObjectStatusChangedEventArg : EventArgs
    {
        public GameObject GameObject { get; private set; }

        public GameObjectStatus Status { get; private set; }

        public GameObjectStatusChangedEventArg(GameObjectStatus newStatus, GameObject obj)
        {
            Status = newStatus;

            GameObject = obj;
        }
    }
}