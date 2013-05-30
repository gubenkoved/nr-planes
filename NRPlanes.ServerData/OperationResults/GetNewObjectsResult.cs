using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;

namespace NRPlanes.ServerData.OperationResults
{
    public class GetNewObjectsResult
    {
        public List<GameObject> Objects;

        public GetNewObjectsResult()
        {
            Objects = new List<GameObject>();
        }
    }
}
