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
        private Engine m_leftDeflectingEngine;
        [DataMember]
        private Engine m_rightDeflectingEngine;

        [DataMember]
        private Engine m_leftForwardEngine;
        [DataMember]
        private Engine m_rightForwardEngine;

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

            m_leftForwardEngine = new RocketEngine(new Vector(-1.55, -6.6), 5, 200000.0) { Info = "Left primary engine" };
            m_rightForwardEngine = new RocketEngine(new Vector(+1.55, -6.6), -5, 200000.0) { Info = "Right primary engine"};

            m_leftDeflectingEngine = new IonEngine(new Vector(+0.6, 2.8), -80, 50000.0) { Info = "Left deflector"};
            m_rightDeflectingEngine = new IonEngine(new Vector(-0.6, 2.8), 80, 50000.0) { Info = "Right deflector"};

            Weapon leftLaserGun = new LaserGun(new Vector(-1.5, -0.4),
                                            0,
                                            TimeSpan.FromMilliseconds(120),
                                            100.0,
                                            30,
                                            1.0,
                                            LaserBullet.Prototype(),
                                            new Vector(0.0, 1.5)) {Info = "Left laser gun"};

            Weapon rightLaserGun = new LaserGun(new Vector(1.5, -0.4),
                                             0,
                                             TimeSpan.FromMilliseconds(120),
                                             100.0,
                                             30,
                                             1.0,
                                             LaserBullet.Prototype(),
                                             new Vector(0.0, 1.5)) { Info = "Right laser gun" };

            Shield shield = new Shield(10.0, 0.5, 1.0, 0.03) { Info = "Shield" };

            AddEquipment(m_leftForwardEngine);
            AddEquipment(m_rightForwardEngine);
            AddEquipment(m_leftDeflectingEngine);
            AddEquipment(m_rightDeflectingEngine);

            AddWeapon(leftLaserGun, WeaponPosition.LeftFront);
            AddWeapon(rightLaserGun, WeaponPosition.RightFront);

            AddEquipment(shield);
        }

        public override void StartMotion(MotionType motion)
        {
            switch (motion)
            {
                case MotionType.Forward:
                    m_leftForwardEngine.TurnOn();
                    m_rightForwardEngine.TurnOn();
                    break;
                case MotionType.Left:
                    m_leftDeflectingEngine.TurnOn();
                    break;
                case MotionType.Right:
                    m_rightDeflectingEngine.TurnOn();
                    break;
                case MotionType.All:
                    m_leftForwardEngine.TurnOn();
                    m_rightForwardEngine.TurnOn();
                    m_leftDeflectingEngine.TurnOn();
                    m_rightDeflectingEngine.TurnOn();
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
                    m_leftForwardEngine.TurnOff();
                    m_rightForwardEngine.TurnOff();
                    break;
                case MotionType.Left:
                    m_leftDeflectingEngine.TurnOff();
                    break;
                case MotionType.Right:
                    m_rightDeflectingEngine.TurnOff();
                    break;
                case MotionType.All:
                    m_leftForwardEngine.TurnOff();
                    m_rightForwardEngine.TurnOff();
                    m_leftDeflectingEngine.TurnOff();
                    m_rightDeflectingEngine.TurnOff();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("motion");
            }
        }
    }
}