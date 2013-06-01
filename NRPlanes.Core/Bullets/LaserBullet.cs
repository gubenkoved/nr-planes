using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Bullets
{
    [DataContract]
    public class LaserBullet : Bullet
    {
        public new static LaserBullet Prototype()
        {
            return Prototype(5.0, 10.0, 20.0, TimeSpan.FromSeconds(20.0));
        }

        public static LaserBullet Prototype(double mass, double angularMass, double power, TimeSpan timeToLive)
        {
            return new LaserBullet(mass, angularMass, new ReferenceArea(0.0), power, timeToLive);
        }

        public LaserBullet(double mass, double angularMass, ReferenceArea referenceArea, double power, TimeSpan timeToLive)
            : base(mass, angularMass, referenceArea, power, timeToLive)
        {
            const double size = 0.35;

            RelativeGeometry = new PolygonGeometry(new[]
                                               {
                                                   new Vector(-size, size),
                                                   new Vector(size, size),
                                                   new Vector(size, -size),
                                                   new Vector(-size, -size)
                                               });
        }
    }
}