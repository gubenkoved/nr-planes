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

        /// <summary>
        /// Returns value in [0; 360)
        /// </summary>
        public static double NormalizeAngle(double angleDegrees)
        {
            double angle = angleDegrees;

            while (angle < 0)
                angle += 360;

            while (angle >= 360)
                angle -= 360;

            return angle;
        }

        /// <summary>
        /// Returns value in range [-180; 180].
        /// <para>Returns 0, then second position in (1, 0) direction (along classical x-axis);</para>
        /// <para>90 deg, then second position in (0, 1) direction (y-axis)</para>
        /// </summary>
        public static double RelativeAngleBetweenPositions(Vector firstPos, Vector secondPos)
        {
            return Vector.AngleBetween(new Vector(1, 0), secondPos - firstPos);
        }
    }
}