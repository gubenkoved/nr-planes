using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Engines
{
    [DataContract]
    public class RocketEngine : Engine
    {
        public RocketEngine(Vector relativePosition, double relativeRotation, double tractionForce)
            : base(tractionForce, 8, 0.30)
        {
            RelativeToOriginPosition = relativePosition;

            RelativeRotation = relativeRotation;

            Size = new Size(1.2, 2.4);
        }
    }
}