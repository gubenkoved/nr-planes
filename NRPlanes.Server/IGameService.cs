using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NRPlanes.Core.Common;
using NRPlanes.ServerData.MutableInformations;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.ServerData.EventsLog;

namespace NRPlanes.Server
{
    [ServiceContract]
    public interface IGameService
    {
        [OperationContract]
        JoinResult Join();

        [OperationContract]
        CommitResult CommitObjects(Guid playerGuid, List<GameObject> objects);

        //[OperationContract]
        //GetNewObjectsResult GetNewObjects(Guid playerGuid, int minId);

        [OperationContract]
        GetEventsLogSinceResult GetEventsLogSince(Guid playerGuid, Timestamp timestamp);

        [OperationContract(IsOneWay = true)]
        void UpdatePlane(Guid playerGuid, PlaneMutableInformation info);

        [OperationContract]
        IEnumerable<PlaneMutableInformation> GetPlanesInfo(Guid playerGuid);
    }
}
