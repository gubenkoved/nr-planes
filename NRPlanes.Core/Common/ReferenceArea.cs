using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    /// <summary>
    /// The reference area is often defined as the area of the orthographic projection of the
    /// object—on a plane perpendicular to the direction of motion—e.g. for objects with a simple
    /// shape, such as a sphere, this is the cross sectional area.
    /// Using linear interpolation to get reference area between specified points.
    /// </summary>
    [DataContract]
    public class ReferenceArea
    {
        [DataMember]
        private List<ReferenceAreaPoint> m_points;

        public ReferenceArea(double constantReferenceArea)
            : this(new[]
                      {
                          new ReferenceAreaPoint(0, constantReferenceArea), 
                          new ReferenceAreaPoint(360.0, constantReferenceArea), 
                      })
        {
        }

        public ReferenceArea(ReferenceAreaPoint[] points)
        {
            if (points.Length < 2)
                throw new Exception("When you use this constructor you must pass at least two known points");

            m_points = points.ToList();

            m_points.Sort();
        }

        /// <returns>Linear interpolation of referece area in point with specified angle</returns>
        public double GetValue(double angle)
        {
            angle = angle % 360;
            angle = angle < 0 ? angle + 360.0 : angle;

            var b = m_points.First(p => p.Angle >= angle);
            var a = m_points.Last(p => p.Angle <= angle);

            if (a == b) return a.ReferenceArea;

            var d = (angle - a.Angle) / (b.Angle - a.Angle);

            // linear interpolation
            return a.ReferenceArea * (1.0 - d) + b.ReferenceArea * d;
        }
    }

    [DataContract]
    public struct ReferenceAreaPoint : IComparable<ReferenceAreaPoint>
    {
        [DataMember]
        public double Angle;

        [DataMember]
        public double ReferenceArea;

        public ReferenceAreaPoint(double angle, double referenceArea)
        {
            Angle = angle;

            if (angle > 360.0)
                Angle = Angle % 360.0;

            ReferenceArea = referenceArea;
        }

        public int CompareTo(ReferenceAreaPoint other)
        {
            if (Angle > other.Angle)
                return 1;

            if (Angle < other.Angle)
                return -1;

            return 0;
        }

        public static bool operator ==(ReferenceAreaPoint one, ReferenceAreaPoint second)
        {
            return one.Angle == second.Angle;
        }

        public static bool operator !=(ReferenceAreaPoint one, ReferenceAreaPoint second)
        {
            return !(one == second);
        }
    }
}