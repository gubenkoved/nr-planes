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
        private List<Vector> _vertexes;
        public ReadOnlyCollection<Vector> Vertexes
        {
            get { return _vertexes.AsReadOnly(); }
        }

        [DataMember]
        private Rect _boundingRectangle;
        public override Rect BoundingRectangle
        {
            get { return _boundingRectangle; }
        }

        [DataMember]
        private List<Segment> _segments;
        public List<Segment> Segments
        {
            get { return _segments; }
        }

        [DataMember]
        private Vector _center;
        public override Vector Center
        {
            get { return _center; }
        }

        public PolygonGeometry(IEnumerable<Vector> vertexes)
        {
            _vertexes = new List<Vector>(vertexes);

            RecalclulateGeometry();
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
            _boundingRectangle = GetBoundingRectangle();

            _segments = GetSegments();

            _center = GetCenter();
        }

        private Rect GetBoundingRectangle()
        {
            double maxX = Vertexes[0].X;
            double minX = Vertexes[0].X;
            double maxY = Vertexes[0].Y;
            double minY = Vertexes[0].Y;

            foreach (var vertex in Vertexes)
            {
                if (vertex.X < minX)
                    minX = vertex.X;

                if (vertex.X > maxX)
                    maxX = vertex.X;

                if (vertex.Y < minY)
                    minY = vertex.Y;

                if (vertex.Y > maxY)
                    maxY = vertex.Y;
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private List<Segment> GetSegments()
        {
            var segments = new List<Segment>();

            for (int i = 1; i < _vertexes.Count; i++)
            {
                segments.Add(new Segment(_vertexes[i - 1], _vertexes[i]));
            }

            segments.Add(new Segment(_vertexes[_vertexes.Count - 1], _vertexes[0]));

            return segments;
        }

        private Vector GetCenter()
        {
            var center = new Vector();

            foreach (var vertex in Vertexes)
            {
                center += vertex;
            }

            center /= Vertexes.Count;

            return center;
        }

        public override bool IsIntersectsWith(Geometry anotherGeometry)
        {
            if (anotherGeometry is PolygonGeometry)
            {
                PolygonGeometry anotherPoligon = (PolygonGeometry)anotherGeometry;

                foreach (var segmentI in Segments)
                {
                    foreach (var segmentJ in anotherPoligon.Segments)
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
            for (int i = 0; i < _vertexes.Count; i++)
            {
                _vertexes[i] += translateVector;
            }

            RecalclulateGeometry();
        }

        public override void Rotate(double rotation)
        {
            for (int i = 0; i < _vertexes.Count; i++)
            {
                _vertexes[i] = _vertexes[i].Rotate(rotation);
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
            return new PolygonGeometry(_vertexes.ToList());
        }
    }
}