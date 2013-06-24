using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Bonuses;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class BonusAppliedLogItem : GameEventsLogItem
    {
        [DataMember]
        private int m_bonusId;
        public int BonusId
        {
            get
            {
                return m_bonusId;
            }
        }

        [DataMember]
        private int m_planeId;
        public int PlaneId
        {
            get
            {
                return m_planeId;
            }
        }

        public BonusAppliedLogItem(Timestamp timestamp, int bonusId, int planeId)
            : base(timestamp)
        {
            m_bonusId = bonusId;
            m_planeId = planeId;
        }
    }
}
