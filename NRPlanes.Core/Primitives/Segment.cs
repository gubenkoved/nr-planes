using System.Runtime.Serialization;
namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public struct Segment
    {
        [DataMember]
        public Vector Start { get; set; }

        [DataMember]
        public Vector End { get; set; }

        public double Length
        {
            get { return (End - Start).Length; }
        }

        public Vector Offset
        {
            get { return End - Start; }
        }

        public Segment(Vector start, Vector end)
            :this()
        {
            Start = start;

            End = end;
        }

        public Vector? IntersectsWith(Segment anotherSegment)
        {
            var a = this;
            var b = anotherSegment;

            var k1 = a.End.X - a.Start.X;
            var k2 = b.Start.X - b.End.X;
            var k3 = b.Start.X - a.Start.X;

            var h1 = a.End.Y - a.Start.Y;
            var h2 = b.Start.Y - b.End.Y;
            var h3 = b.Start.Y - a.Start.Y;

            var d = k1 * h2 - h1 * k2;
            var da = k3 * h2 - h3 * k2;
            var db = k1 * h3 - h1 * k3;

            var ta = da / d;
            var tb = db / d;

            if (ta >= 0.0 && ta <= 1.0 && tb >= 0.0 && tb <= 1.0)
            {
                var intersectionVector = a.Start + ta * (a.End - a.Start);
                
                return intersectionVector;
            }
            else
            {
                return null;
            }
        }
    }
}