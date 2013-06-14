namespace NRPlanes.Core.Primitives
{
    public static class Extensions
    {
        //public static Vector Rotate(this Vector vector, double angle)
        //{
        //    var rotationMatrix = Matrix.CreateRotationMatrix(angle);

        //    return Vector.Multiply(vector, rotationMatrix);
        //}

        public static double Angle(this Vector vector)
        {
            return Vector.AngleBetween(new Vector(0, 1), vector);
        }        
    }
}