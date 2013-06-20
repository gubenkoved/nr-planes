using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Core.Engines
{
    [DataContract]
    public class RocketEngine : Engine
    {
        public RocketEngine(double tractionForce)
            : base(tractionForce, 8, 0.30)
        {            
            Size = new Size(1.2, 2.4);
        }
    }
}