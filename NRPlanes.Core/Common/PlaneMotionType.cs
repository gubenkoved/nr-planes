using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    public enum PlaneMotionType
    {
        Forward,
        Left,
        Right,
        All
    }
}
