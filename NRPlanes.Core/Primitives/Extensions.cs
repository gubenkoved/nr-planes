namespace NRPlanes.Core.Primitives
{
    public static class Extensions
    {
        public static Vector Rotate(this Vector vector, double angle)
        {
            var rotationMatrix = Matrix.CreateRotationMatrix(angle);

            return Vector.Multiply(vector, rotationMatrix);
        }

        public static double Angle(this Vector vector)
        {
            return Vector.AngleBetween(vector, new Vector(0, 1.0));
        }

        public static Vector Ort(this Vector vector)
        {
            if (vector.Length == 0.0)
                return new Vector();
            else
                return vector / vector.Length;
        }

        public static bool HitTest(this Rect rect, Vector vector)
        {
            if (rect.X <= vector.X && rect.X + rect.Width >= vector.X
                && rect.Y <= vector.Y && rect.Y + rect.Height >= vector.Y)
                return true;

            return false;
        }
    }
}