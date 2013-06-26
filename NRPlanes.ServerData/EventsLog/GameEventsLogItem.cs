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
    [KnownType(typeof(BonusAppliedLogItem))]
    [KnownType(typeof(GameObjectExplodedLogItem))]
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

        [DataMember]
        private bool m_isDepreciated;
        /// <summary>
        /// Depritiated items doen not needed to new players
        /// </summary>
        public bool IsDepriciated
        {
            get
            {
                return m_isDepreciated;
            }
            set
            {
                m_isDepreciated = value;
            }
        }

        public GameEventsLogItem(Timestamp timestamp)
        {
            m_timestamp = timestamp;
        }
    }
}
