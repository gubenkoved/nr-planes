using System.Runtime.Serialization;
namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public struct Size
    {
        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        public double Aspect
        {
            get { return Width / Height; }
        }

        public Size(double width, double height)
            :this()
        {
            Width = width;

            Height = height;
        }

        public static Size operator *(Size s, double d)
        {
            return new Size(d * s.Width, d * s.Height);
        }
        public static Size operator *(double d, Size s)
        {
            return s * d;
        }
        public static Size operator -(Size s1, Size s2)
        {
            return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
        }

        public override string ToString()
        {
            return string.Format("{{{0}x{1}}}", Width, Height);
        }
    }
}