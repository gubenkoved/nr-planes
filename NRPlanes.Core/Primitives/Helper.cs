using System;

namespace NRPlanes.Core.Primitives
{
    public static class Helper
    {
         internal static bool RangeIntersects(double start1, double len1, double start2, double len2)
         {
             if (start1 < start2 && start1 + len1 >= start2)
                 return true;

             if (start2 < start1 && start2 + len2 >= start1)
                 return true;

             if (start1 == start2)
                 return true;

             return false;
         }

        public static double ToRadians(double degrees)
        {
            return Math.PI * (degrees % 360) / 180.0;
        }

        public static double ToDegrees(double radians)
        {
            return (radians * 180.0 / Math.PI) % 360;
        }
    }
}