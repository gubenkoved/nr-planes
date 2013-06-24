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
        public readonly GameObject GameObject;

        public readonly GameObjectStatus Status;

        public GameObjectStatusChangedEventArg(GameObjectStatus newStatus, GameObject obj)
        {
            Status = newStatus;

            GameObject = obj;
        }
    }
}