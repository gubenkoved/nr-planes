using System;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Common;
using NRPlanes.Core.Engines;
using NRPlanes.Core.Weapons;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;

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

            m_leftForwardEngine = new RocketEngine(200000.0) { Info = "Left primary engine" };
            m_rightForwardEngine = new RocketEngine(200000.0) { Info = "Right primary engine"};

            m_leftDeflectingEngine = new IonEngine(50000.0) { Info = "Left deflector"};
            m_rightDeflectingEngine = new IonEngine(50000.0) { Info = "Right deflector"};

            Weapon leftLaserGun = LaserGun.CreateDefault();
            leftLaserGun.Info = "Left laser gun";

            Weapon rightLaserGun = LaserGun.CreateDefault();
            rightLaserGun.Info = "Right laser gun";

            Shield shield = new Shield(10.0, 0.5, 1.0, 0.03) { Info = "Shield" };

            AddEquipment(m_leftForwardEngine, new PlaneEquipmentRelativeInfo() { RelativeToOriginPosition = new Vector(-1.55, -6.6), RelativeRotation = 5 });
            AddEquipment(m_rightForwardEngine, new PlaneEquipmentRelativeInfo() { RelativeToOriginPosition = new Vector(+1.55, -6.6), RelativeRotation = -5 });
            AddEquipment(m_leftDeflectingEngine, new PlaneEquipmentRelativeInfo() { RelativeToOriginPosition = new Vector(+0.6, 2.8), RelativeRotation = -80 });
            AddEquipment(m_rightDeflectingEngine, new PlaneEquipmentRelativeInfo() { RelativeToOriginPosition = new Vector(-0.6, 2.8), RelativeRotation = 80 });

            AddEquipment(leftLaserGun, new PlaneWeaponRelativeInfo() { WeaponPosition = WeaponPosition.LeftFront, RelativeToOriginPosition = new Vector(-1.5, -0.4), RelativeRotation = 0 });
            AddEquipment(rightLaserGun, new PlaneWeaponRelativeInfo() { WeaponPosition = WeaponPosition.LeftFront, RelativeToOriginPosition = new Vector(1.5, -0.4), RelativeRotation = 0 });

            AddEquipment(shield, new PlaneEquipmentRelativeInfo());
        }

        public override void StartMotion(PlaneMotionType motion)
        {
            switch (motion)
            {
                case PlaneMotionType.Forward:
                    m_leftForwardEngine.TurnOn();
                    m_rightForwardEngine.TurnOn();
                    break;
                case PlaneMotionType.Left:
                    m_leftDeflectingEngine.TurnOn();
                    break;
                case PlaneMotionType.Right:
                    m_rightDeflectingEngine.TurnOn();
                    break;
                case PlaneMotionType.All:
                    m_leftForwardEngine.TurnOn();
                    m_rightForwardEngine.TurnOn();
                    m_leftDeflectingEngine.TurnOn();
                    m_rightDeflectingEngine.TurnOn();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("motion");
            }
        }

        public override void EndMotion(PlaneMotionType motion)
        {
            switch (motion)
            {
                case PlaneMotionType.Forward:
                    m_leftForwardEngine.TurnOff();
                    m_rightForwardEngine.TurnOff();
                    break;
                case PlaneMotionType.Left:
                    m_leftDeflectingEngine.TurnOff();
                    break;
                case PlaneMotionType.Right:
                    m_rightDeflectingEngine.TurnOff();
                    break;
                case PlaneMotionType.All:
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