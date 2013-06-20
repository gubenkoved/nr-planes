using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Core.Equipments;

namespace NRPlanes.ServerData
{
    /// <summary>
    /// Must be used to preprocess the game object, which was recieved from the network.
    /// Some of data was excluded from serialization for some reasons.
    /// This class is designed to recover excluded data
    /// </summary>
    public static class IntegrityDataHelper
    {
        public static GameObject PreprocessRecieved(GameObject gameObject)
        {
            // RelatedGameObject was excluded from Equipment serialization to avoid cycle object graph
            if (gameObject is IHaveEquipment)
            {
                
                foreach (var equipment in (gameObject as IHaveEquipment).AllEquipment)
                {
                    equipment.RelatedGameObject = gameObject;
                }
            }

            return gameObject;
        }
    }
}
