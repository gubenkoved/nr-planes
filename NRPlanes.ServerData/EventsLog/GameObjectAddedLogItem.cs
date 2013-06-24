using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class GameObjectAddedLogItem : GameEventsLogItem
    {
        [DataMember]
        private readonly GameObject m_gameObject;
        public GameObject GameObject
        {
            get
            {
                return m_gameObject;
            }
        }

        public GameObjectAddedLogItem(Timestamp timestamp, GameObject obj)
            : base(timestamp)
        {
            m_gameObject = obj;
        }
    }
}
