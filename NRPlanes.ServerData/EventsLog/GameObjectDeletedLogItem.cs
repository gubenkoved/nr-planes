using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class GameObjectDeletedLogItem : GameEventsLogItem
    {
        [DataMember]
        private readonly int m_gameObjectId;
        public int GameObjectId
        {
            get
            {
                return m_gameObjectId;
            }
        }

        public GameObjectDeletedLogItem(Timestamp timestamp, GameObject obj)
            : base(timestamp)
        {
            m_gameObjectId = obj.Id.Value;
        }
    }
}
