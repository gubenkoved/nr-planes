using System;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    //[DataContract]
    //public struct Matrix
    //{
    //    [DataMember]
    //    private double _m11;
    //    [DataMember]
    //    private double _m12;
    //    [DataMember]
    //    private double _m21;
    //    [DataMember]
    //    private double _m22;

    //    public double M11 { get { return _m11; } set { _m11 = value; } }
    //    public double M12 { get { return _m12; } set { _m12 = value; } }
    //    public double M21 { get { return _m21; } set { _m21 = value; } }
    //    public double M22 { get { return _m22; } set { _m22 = value; } }

    //    public Matrix(double m11, double m12, double m21, double m22)
    //    {
    //        _m11 = m11;
    //        _m12 = m12;
    //        _m21 = m21;
    //        _m22 = m22;
    //    }

    //    /// <param name="angle">Clockwise rotation angle in dergrees</param>
    //    /// <returns>Rotation matrix</returns>
    //    public static Matrix CreateRotationMatrix(double angle)
    //    {
    //        double radians = Helper.ToRadians(angle);

    //        double sin = Math.Sin(radians);
    //        double cos = Math.Cos(radians);

    //        return new Matrix(cos, -sin, sin, cos);
    //    }
    //}
}