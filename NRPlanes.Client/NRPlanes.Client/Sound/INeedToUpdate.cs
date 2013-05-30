using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Client.Sound
{
    /// <summary>
    /// All sound effects marked by this interface will be updated when SoundManager Update() method is invoked
    /// </summary>
    public interface INeedToUpdate
    {
        void Update(TimeSpan elapsed);
    }
}
