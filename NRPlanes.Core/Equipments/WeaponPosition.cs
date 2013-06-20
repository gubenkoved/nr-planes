using System.Runtime.Serialization;

namespace NRPlanes.Core.Equipments
{
    [DataContract(Name = "WeaponPositionEnum")]
    public enum WeaponPosition
    {
        [EnumMember]
        LeftFront,
        [EnumMember]
        CenterFront,
        [EnumMember]
        RightFront,
        [EnumMember]
        Unknown
    }
}