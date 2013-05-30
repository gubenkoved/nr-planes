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

        public override string ToString()
        {
            return string.Format("{{{0}x{1}}}", Width, Height);
        }
    }
}