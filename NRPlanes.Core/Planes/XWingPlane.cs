using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Common;
using NRPlanes.Core.Engines;
using NRPlanes.Core.Weapons;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Planes
{
    [DataContract]
    public class XWingPlane : Plane
    {
        [DataMember]
        private Engine _leftDeflectingEngine;
        [DataMember]
        private Engine _rightDeflectingEngine;

        [DataMember]
        private Engine _leftForwardEngine;
        [DataMember]
        private Engine _rightForwardEngine;

        public static XWingPlane BasicConfiguration(Vector initialPosition)
        {
            var mass = 10000.0;
            var angularMass = 1000.0;

            var referenceArea = new ReferenceArea
                (new[] 
                {
                    new ReferenceAreaPoint(0, 1000),
                    new ReferenceAreaPoint(90, 2000),
                    new ReferenceAreaPoint(180, 4000),
                    new ReferenceAreaPoint(270, 2000),
                    new ReferenceAreaPoint(360, 1000)
                });

            return new XWingPlane(mass, angularMass, referenceArea, initialPosition);
        }

        public XWingPlane(double mass, double angularMass, ReferenceArea referenceArea, Vector initialPosition)
            :base(mass, angularMass, referenceArea, 1000)
        {
            Position = initialPosition;

            RelativeGeometry = new PolygonGeometry(new[]
                                                       {
                                                           new Vector(0, 6),
                                                           new Vector(0.5, -0.6),
                                                           new Vector(7, -1),
                                                           new Vector(7, -5),
                                                           new Vector(-7, -5),
                                                           new Vector(-7, -1),
                                                           new Vector(-0.5, -0.6)
                                                       });

            _leftForwardEngine = new RocketEngine(new Vector(-1.55, -6.6), 5, 200000.0) { Info = "Left primary engine" };
            _rightForwardEngine = new RocketEngine(new Vector(+1.55, -6.6), -5, 200000.0) { Info = "Right primary engine"};

            _leftDeflectingEngine = new IonEngine(new Vector(+0.6, 2.8), -80, 50000.0) { Info = "Left deflector"};
            _rightDeflectingEngine = new IonEngine(new Vector(-0.6, 2.8), 80, 50000.0) { Info = "Right deflector"};

            Weapon leftLaserGun = new LaserGun(new Vector(-1.5, -0.4),
                                            0,
                                            WeaponPosition.LeftFront,
                                            TimeSpan.FromMilliseconds(120),
                                            100.0,
                                            30,
                                            1.0,
                                            LaserBullet.Prototype(),
                                            new Vector(0.0, 1.5)) {Info = "Left laser gun"};

            Weapon rightLaserGun = new LaserGun(new Vector(1.5, -0.4),
                                             0,
                                             WeaponPosition.RightFront,
                                             TimeSpan.FromMilliseconds(120),
                                             100.0,
                                             30,
                                             1.0,
                                             LaserBullet.Prototype(),
                                             new Vector(0.0, 1.5)) { Info = "Right laser gun" };

            Shield shield = new Shield(10.0, 0.5, 1.0, 0.03) { Info = "Shield" };

            AddEquipment(_leftForwardEngine);
            AddEquipment(_rightForwardEngine);
            AddEquipment(_leftDeflectingEngine);
            AddEquipment(_rightDeflectingEngine);

            AddEquipment(leftLaserGun);
            AddEquipment(rightLaserGun);

            AddEquipment(shield);
        }

        public override void StartMotion(MotionType motion)
        {
            switch (motion)
            {
                case MotionType.Forward:
                    _leftForwardEngine.TurnOn();
                    _rightForwardEngine.TurnOn();
                    break;
                case MotionType.Left:
                    _leftDeflectingEngine.TurnOn();
                    break;
                case MotionType.Right:
                    _rightDeflectingEngine.TurnOn();
                    break;
                case MotionType.All:
                    _leftForwardEngine.TurnOn();
                    _rightForwardEngine.TurnOn();
                    _leftDeflectingEngine.TurnOn();
                    _rightDeflectingEngine.TurnOn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("motion");
            }
        }

        public override void EndMotion(MotionType motion)
        {
            switch (motion)
            {
                case MotionType.Forward:
                    _leftForwardEngine.TurnOff();
                    _rightForwardEngine.TurnOff();
                    break;
                case MotionType.Left:
                    _leftDeflectingEngine.TurnOff();
                    break;
                case MotionType.Right:
                    _rightDeflectingEngine.TurnOff();
                    break;
                case MotionType.All:
                    _leftForwardEngine.TurnOff();
                    _rightForwardEngine.TurnOff();
                    _leftDeflectingEngine.TurnOff();
                    _rightDeflectingEngine.TurnOff();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("motion");
            }
        }
    }
}