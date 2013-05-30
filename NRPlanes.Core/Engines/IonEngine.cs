using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Engines
{
    [DataContract]
    public class IonEngine : Engine
    {
        public IonEngine(Vector relativePosition, double relativeRotation, double tractionForce)
            : base(tractionForce, 10.0, 0.15)
        {
            RelativeToOriginPosition = relativePosition;

            RelativeRotation = relativeRotation;

            Size = new Size(0.6, 1.5);
        }
    }
}