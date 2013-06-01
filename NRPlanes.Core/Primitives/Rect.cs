using System;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public struct Rect
    {
        /// <summary>
        /// X coordinate of left bottom point
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of left bottom point
        /// </summary>
        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public double Height { get; set; }

        public double Area
        {
            get
            {
                return Width * Height;
            }
        }

        /// <param name="x">X coordinate of left bottom corner</param>
        /// <param name="y">Y coordinate of left bottom corner</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rect(double x, double y, double width, double height)
            :this()
        {
            X = x;
            
            Y = y;
            
            Width = width;

            Height = height;
        }

        public Rect(Size size)
            : this(0.0, 0.0, size.Width, size.Height)
        {
        }

        public Rect(Vector center, Size size)
            :this(center.X - size.Width / 2.0, center.Y - size.Height / 2.0, size.Width, size.Height)
        {
        }

        public bool IntersectsWith(Rect rect)
        {
            if (Helper.RangeIntersects(X, Width, rect.X, rect.Width) 
                && Helper.RangeIntersects(Y, Height, rect.Y, rect.Height))
                return true;

            return false;
        }

        public void Scale(double scale)
        {
            double newWidth = Width * scale;
            double newHeight = Height * scale;

            X -= (newWidth - Width) / 2.0;
            Y -= (newHeight - Height) / 2.0;

            Width = newWidth;
            Height = newHeight;
        }
        
        public Vector TopLeft
        {
            get
            {
                return new Vector(X, Y + Height);
            }
        }

        public Vector BottomLeft
        {
            get
            {
                return new Vector(X, Y);
            }
        }

        public Vector TopRight
        {
            get
            {
                return new Vector(X + Width, Y + Height);
            }
        }

        public Vector BottomRight
        {
            get
            {
                return new Vector(X + Width, Y);
            }
        }

        public double Left
        {
            get { return X; }
        }

        public double Right
        {
            get { return X + Width; }
        }

        public double Top
        {
            get { return Y + Height; }
        }

        public double Bottom
        {
            get { return Y; }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
        }        

        public override string ToString()
        {
            return string.Format("({0}, {1}) {{{2}x{3}}}", X, Y, Width, Height);
        }

        public double LongSide
        {
            get { return Math.Max(Width, Height); }
        }

        public double ShortSide
        {
            get { return Math.Min(Width, Height); }
        }

        public Vector Center
        {
            get { return new Vector(X + Width / 2.0, Y + Height / 2.0); }
        }
    }
}