using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.Common;

namespace NRPlanes.ServerData.MutableInformations
{
    [DataContract]
    [KnownType(typeof(PlaneMutableInformation))]
    public class GameObjectMutableInformation : MutableInformation
    {
        [DataMember]
        public int? Id;

        [DataMember]
        public double Rotation;

        [DataMember]
        public double RotationVelocity;

        [DataMember]
        public Vector Position;

        [DataMember]
        public Vector Velocity;

        private static PrivateValueAccessor m_rotationPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Rotation");
        private static PrivateValueAccessor m_rotationVelocityPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "RotationVelocity");
        private static PrivateValueAccessor m_positionPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Position");
        private static PrivateValueAccessor m_velocityPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Velocity");

        public GameObjectMutableInformation(GameObject obj)
        {
            Id = obj.Id; // for game object identification if it needed

            Rotation = obj.Rotation;
            RotationVelocity = obj.RotationVelocity;
            Position = obj.Position;
            Velocity = obj.Velocity;
        }

        public override void Apply(object obj)
        {
            m_rotationPrivateFieldAccessor.SetValue(obj, Rotation);
            m_rotationVelocityPrivateFieldAccessor.SetValue(obj, RotationVelocity);
            m_positionPrivateFieldAccessor.SetValue(obj, Position);
            m_velocityPrivateFieldAccessor.SetValue(obj, Velocity);
        }

        public override string ToString()
        {
            return string.Format("ID:{0}, Rot:{1:F3}, RotV:{2:F3}, Pos:{3}, V:{4}", Id, Rotation, RotationVelocity, Position, Velocity);
        }
    }
}
