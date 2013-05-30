using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.OperationResults
{
    [DataContract]
    public class CommitResult
    {
        [DataMember]
        public List<int> ObjectsIds;
    }
}
