using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public class PolygonGeometry : Geometry
    {
        [DataMember]
        private Vector[] m_vertexes;
        public ReadOnlyCollection<Vector> Vertexes
        {
            get 
            {
                return Array.AsReadOnly<Vector>(m_vertexes);
            }
        }

        [DataMember]
        private Rect m_boundingRectangle;
        public override Rect BoundingRectangle
        {
            get 
            { 
                return m_boundingRectangle; 
            }
        }

        [DataMember]
        private Segment[] m_segments;
        public ReadOnlyCollection<Segment> Segments
        {
            get 
            {
                return Array.AsReadOnly<Segment>(m_segments);
            }
        }

        [DataMember]
        private Vector m_center;
        public override Vector Center
        {
            get 
            { 
                return m_center; 
            }
        }

        public PolygonGeometry(IEnumerable<Vector> vertexes)
        {
            m_vertexes = vertexes.ToArray();

            RecalclulateGeometry();
        }

        internal PolygonGeometry(Vector[] vertexes, Segment[] segments, Vector center, Rect boundingRect)
        {
            m_vertexes = vertexes;
            m_segments = segments;
            m_center = center;
            m_boundingRectangle = boundingRect;
        }

        public static PolygonGeometry FromRectangle(Rect rect)
        {
            return new PolygonGeometry
                (new[] 
                {
                    new Vector(rect.X, rect.Y),
                    new Vector(rect.X + rect.Width, rect.Y),
                    new Vector(rect.X + rect.Width, rect.Y + rect.Height),
                    new Vector(rect.X, rect.Y + rect.Height)
                });

        }

        private void RecalclulateGeometry()
        {
            m_segments = new Segment[m_vertexes.Length];

            Vector sum = m_vertexes[0];
            Vector max = Vertexes[0];
            Vector min = Vertexes[0];

            for (int i = 1; i < m_vertexes.Length; i++)
            {
                Vector vertex = m_vertexes[i];

                min.X = vertex.X < min.X ? vertex.X : min.X;
                min.Y = vertex.Y < min.Y ? vertex.Y : min.Y;

                max.X = vertex.X > max.X ? vertex.X : max.X;
                max.Y = vertex.Y > max.Y ? vertex.Y : max.Y;

                sum += vertex;

                m_segments[i - 1] = new Segment(m_vertexes[i - 1], m_vertexes[i]);
            }

            m_segments[m_vertexes.Length - 1] = new Segment(m_vertexes[m_vertexes.Length - 1], m_vertexes[0]);
            m_boundingRectangle = new Rect(min.X, min.Y, max.X - min.X, max.Y - min.Y);
            m_center = sum / m_vertexes.Length;
        }

        public override bool IsIntersectsWith(Geometry anotherGeometry)
        {
            if (anotherGeometry is PolygonGeometry)
            {
                PolygonGeometry anotherPoligon = (PolygonGeometry)anotherGeometry;

                foreach (var segmentI in m_segments)
                {
                    foreach (var segmentJ in anotherPoligon.m_segments)
                    {
                        if (segmentI.IntersectsWith(segmentJ) != null)
                            return true;
                    }
                }

                return false;
            }
            else
            {
                return base.IsIntersectsWith(anotherGeometry);
            }
        }

        public override void Translate(Vector translateVector)
        {
            for (int i = 0; i < m_vertexes.Length; i++)
            {
                m_vertexes[i] += translateVector;
            }

            RecalclulateGeometry();
        }

        public override void Rotate(double rotation)
        {
            for (int i = 0; i < m_vertexes.Length; i++)
            {
                m_vertexes[i] = m_vertexes[i].Rotate(rotation);
            }

            RecalclulateGeometry();
        }

        public override bool HitTest(Vector vector)
        {
            var numOfIntersections = 0;
            var ray = new Segment(vector, new Vector(1000000, 0));

            foreach (var segment in Segments)
            {
                if (segment.IntersectsWith(ray) != null)
                    ++numOfIntersections;
            }

            return numOfIntersections % 2 == 1;
        }

        public override Geometry Clone()
        {
            return new PolygonGeometry(
                (Vector[])m_vertexes.Clone(),
                (Segment[])m_segments.Clone(),
                m_center,
                m_boundingRectangle);
        }
    }
}