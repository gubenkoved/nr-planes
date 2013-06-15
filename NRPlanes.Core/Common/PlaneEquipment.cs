using System;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    [DataContract]
    [KnownType(typeof(Weapon))]
    [KnownType(typeof(Engine))]
    [KnownType(typeof(Shield))]
    public abstract class PlaneEquipment : Equipment<Plane>, IUpdatable
    {
        protected PlaneEquipment(double maximumCharge, double regeneration)
        {
            MaximumCharge = maximumCharge;

            Charge = MaximumCharge;

            Regeneration = regeneration;
        }

        /// <summary>
        /// Maximum value of charge
        /// </summary>
        [DataMember]
        public double MaximumCharge { get; protected set; }

        [DataMember]
        public double Charge { get; protected set; }

        [DataMember]
        public double MinimalChargeToActivate { get; protected set; }

        public virtual bool IsLowCharge
        {
            get
            {
                return Charge < MaximumCharge / 10.0;
            }
        }

        /// <summary>
        /// Time during restores one unit of charge
        /// </summary>
        [DataMember]
        public double Regeneration { get; protected set; }

        public virtual void Update(TimeSpan elapsed)
        {
            Charge = Math.Min(MaximumCharge, Charge + Regeneration * elapsed.TotalSeconds);
        }
    }
}