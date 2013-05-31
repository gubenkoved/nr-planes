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
        /// Returns value in range [-180; 180]:
        /// <para>- Negative value ((0;-180))  when rotation is counterclock-wise</para>
        /// <para>- Positive value ([0;180]) when rotation is clock-wise</para>
        /// </summary>
        public static double RelativeAngleBetweenPositions(Vector from, Vector to)
        {
            return Vector.AngleBetween(new Vector(0, 1), to - from);
        }

        /// <summary>
        /// Returns vector angle value [-180; 180]:
        /// <para>- Negative value ((0;-180)) when vector in left half-plane (with negative x)</para>
        /// <para>- Positive value ([0;180]) when vector in right half-plane (with positive x)</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double Angle(Vector vector)
        {
            return Vector.AngleBetween(new Vector(0, 1), vector);
        }
    }
}