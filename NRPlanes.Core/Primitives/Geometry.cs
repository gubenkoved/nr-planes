using System;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    /// <summary>
    /// Geometry of any object on game field
    /// </summary>
    [DataContract]
    [KnownType(typeof(PolygonGeometry))]
    [KnownType(typeof(CircleGeometry))]    
    public abstract class Geometry
    {
        /// <summary>
        /// Bounding rectangle
        /// </summary>
        public abstract Rect BoundingRectangle { get; }

        /// <summary>
        /// Center on geometry
        /// </summary>
        public abstract Vector Center { get; }

        /// <summary>
        /// Basic implementation based on bounding rectangles intersection
        /// </summary>
        public virtual bool IsIntersectsWith(Geometry anotherGeometry)
        {
            return BoundingRectangle.IntersectsWith(anotherGeometry.BoundingRectangle);
        }

        public bool IsIntersectsOrInclude(Geometry anotherGeomery)
        {
            if (IsIntersectsWith(anotherGeomery))
                return true;

            if (BoundingRectangle.HitTest(anotherGeomery.Center) && HitTest(anotherGeomery.Center)) // another geomenty in this geometry case
                return true;

            if (anotherGeomery.BoundingRectangle.HitTest(Center) && anotherGeomery.HitTest(Center)) // this geomenty in another geometry case
                return true;

            return false;
        }

        public virtual bool HitTest(Vector vector)
        {
            // simplest implementation by default

            return BoundingRectangle.HitTest(vector);
        }

        public abstract void Translate(Vector translateVector);

        public abstract void Rotate(double rotation);

        public abstract Geometry Clone();
    }
}