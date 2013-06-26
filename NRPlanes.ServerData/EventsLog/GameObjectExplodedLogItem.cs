using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class GameObjectExplodedLogItem : GameEventsLogItem
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

        public GameObjectExplodedLogItem(Timestamp timestamp, int explodedObjId)
            :base(timestamp)
        {
            m_gameObjectId = explodedObjId;
        }
    }
}
