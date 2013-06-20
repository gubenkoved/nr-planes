using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Bullets
{
    [DataContract]
    public class LaserBullet : Bullet
    {
        public static LaserBullet Default
        {
            get
            {
                return new LaserBullet(
                    mass: 5.0,
                    angularMass: 10.0,
                    power: 20.0,
                    timeToLive: TimeSpan.FromSeconds(20.0));
            }
        }        

        private LaserBullet(double mass, double angularMass, double power, TimeSpan timeToLive)
            : base(mass, angularMass, new ReferenceArea(0.0), power, timeToLive)
        {
            const double radius = 0.35;

            //RelativeGeometry = new CircleGeometry(new Vector(0, 0), radius);

            RelativeGeometry = new PolygonGeometry(new[]
                                               {
                                                   new Vector(-radius, radius),
                                                   new Vector(radius, radius),
                                                   new Vector(radius, -radius),
                                                   new Vector(-radius, -radius)
                                               });
        }

        
    }
}