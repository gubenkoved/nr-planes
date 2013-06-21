using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Core.Common
{
    public static class RandomProvider
    {
        static private Random m_random = new Random(Environment.TickCount);

        /// <summary>
        /// Returns random double from 0.0 to 1.0
        /// </summary>
        static public double NextDouble()
        {
            return m_random.NextDouble();
        }

        /// <summary>
        /// Returns random nonnegative integer
        /// </summary>
        static public int Next()
        {
            return m_random.Next();
        }
    }
}
