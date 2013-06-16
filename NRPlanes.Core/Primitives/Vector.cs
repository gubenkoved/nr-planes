using System;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    [DataContract]
    public struct Vector
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        public Vector(double x, double y)
            :this()
        {
            X = x;
            Y = y;
        }

        public double Length
        {
            get { return Math.Sqrt(LengthSquared); }
        }
        public double LengthSquared
        {
            get { return X * X + Y * Y; }
        }
        
        static public Vector Zero
        {
            get
            {
                return new Vector(0, 0);
            }
        }

        /// <summary>
        /// Returns reflected by x-axis vector (y -> -y)
        /// </summary>
        public Vector ReflectionByXAxis()
        {
            return new Vector(X, -Y);
        }

        /// <summary>
        /// Returns reflected by y-axis vector (x -> -x)
        /// </summary>
        public Vector ReflectionByYAxis()
        {
            return new Vector(-X, Y);
        }

        public Vector Ort()
        {
            if (LengthSquared == 0.0)
                return new Vector(0, 0);
            else
                return this / this.Length;
        }

        public Vector Rotate(double angleDegrees)
        {
            double radians = Helper.ToRadians(angleDegrees);

            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);

            return new Vector(
                X * cos + Y * sin,
                -X * sin + Y * cos);
        }        

        static public Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }
        static public Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }
        static public Vector operator *(Vector v, double d)
        {
            return new Vector(d * v.X, d * v.Y);
        }
        static public Vector operator *(double d, Vector v)
        {
            return v * d;
        }
        static public double operator *(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        static public Vector operator /(Vector v, double d)
        {
            return new Vector(v.X / d, v.Y / d);
        }
        static public Vector operator - (Vector v)
        {
            return -1.0 * v;
        }

        /// <summary>
        /// Angle between vectors in degrees (-180; 180]
        /// </summary>
        /// <returns>Value in range (-180; 180]</returns>
        static public double AngleBetween(Vector v1, Vector v2)
        {
            // from System.Windows AngleBetween

            double y = v1.X * v2.Y - v2.X * v1.Y;
            double x = v1.X * v2.X + v1.Y * v2.Y;
            
            return Helper.ToDegrees(Math.Atan2(-y, x));
        }

        //public static Vector Multiply(Vector v, Matrix matrix)
        //{
        //    double x = v.X * matrix.M11 + v.Y * matrix.M21;
        //    double y = v.X * matrix.M12 + v.Y * matrix.M22;

        //    return new Vector(x, y);
        //}

        public override string ToString()
        {
            return string.Format("({0:F1}; {1:F1})", X, Y);
        }
    }
}