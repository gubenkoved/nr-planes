using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    [KnownType(typeof(GameObjectAddedLogItem))]
    [KnownType(typeof(GameObjectDeletedLogItem))]
    public abstract class GameEventsLogItem
    {
        [DataMember]
        private readonly Timestamp m_timestamp;
        public Timestamp Timestamp
        {
            get
            {
                return m_timestamp;
            }
        }

        public GameEventsLogItem(Timestamp timestamp)
        {
            m_timestamp = timestamp;
        }
    }
}
