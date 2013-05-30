using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;

namespace NRPlanes.ServerData.OperationResults
{
    /// <summary>
    /// This data has been send to client when his join to server
    /// </summary>
    [DataContract]
    public class JoinResult
    {
        [DataMember]
        public Guid PlayerGuid;

        #region Initial GameWorld parameters
        [DataMember]
        public Size LogicalSize;

        [DataMember]
        public IEnumerable<StaticObject> StaticObjects;
        #endregion
    }
}
