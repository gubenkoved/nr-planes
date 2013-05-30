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

        private static PrivateValueAccessor _rotationPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Rotation");
        private static PrivateValueAccessor _rotationVelocityPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "RotationVelocity");
        private static PrivateValueAccessor _positionPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Position");
        private static PrivateValueAccessor _velocityPrivateFieldAccessor = new PrivateValueAccessor(typeof(GameObject), "Velocity");

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
            _rotationPrivateFieldAccessor.SetValue(obj, Rotation);
            _rotationVelocityPrivateFieldAccessor.SetValue(obj, RotationVelocity);
            _positionPrivateFieldAccessor.SetValue(obj, Position);
            _velocityPrivateFieldAccessor.SetValue(obj, Velocity);
        }
    }
}
