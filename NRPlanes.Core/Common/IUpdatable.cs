using System;

namespace NRPlanes.Core.Common
{
    public interface IUpdatable
    {
        void Update(TimeSpan elapsed);
    }
}