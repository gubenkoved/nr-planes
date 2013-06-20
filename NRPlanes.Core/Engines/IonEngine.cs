using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Core.Engines
{
    [DataContract]
    public class IonEngine : Engine
    {
        public IonEngine(double tractionForce)
            : base(tractionForce, 10.0, 0.15)
        {
            Size = new Size(0.6, 1.5);
        }
    }
}