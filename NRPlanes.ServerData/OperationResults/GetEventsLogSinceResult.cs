using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.ServerData.EventsLog;

namespace NRPlanes.ServerData.OperationResults
{
    [DataContract]
    public class GetEventsLogSinceResult
    {
        [DataMember]
        public IEnumerable<GameEventsLogItem> LogItems;
    }
}
