using System;

namespace NRPlanes.Core.Equipments
{
    public enum GameObjectEquipmentStatus
    {
        Added,
        Removed
    }

    public class GameObjectEquipmentStatusChangedArgs : EventArgs    
    {
        public readonly Equipment Equipment;
        public readonly GameObjectEquipmentStatus Status;

        public GameObjectEquipmentStatusChangedArgs(GameObjectEquipmentStatus newStatus, Equipment equipment)
        {
            Status = newStatus;
            Equipment = equipment;
        }
    }
}