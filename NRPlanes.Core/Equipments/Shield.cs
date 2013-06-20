using System.Runtime.Serialization;
using System;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    public class Shield : PlaneEquipment
    {
        [DataMember]
        public bool IsActive { get; private set; }

        [DataMember]
        public double DamageToChargeFactor { get; private set; }

        public Shield(double maximumCharge, double regeneration, double minimalChargeToActivate, double damageToChargeFactor)
            : base(maximumCharge, regeneration)
        {
            MinimalChargeToActivate = minimalChargeToActivate;
            
            DamageToChargeFactor = damageToChargeFactor;
        }

        public void TurnOn()
        {
            if (Charge >= MinimalChargeToActivate)
                IsActive = true;
        }

        public void TurnOff()
        {
            IsActive = false;
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            if (IsActive)
            {
                Charge = Math.Max(0.0, Charge - elapsed.TotalSeconds);

                if (Charge == 0.0)
                    IsActive = false;
            }
        }

        /// <summary>
        /// Returns finite damage value after shield absorpsion
        /// </summary>
        internal double CalculateEffectiveDamage(double initPower)
        {
            if (IsActive)
            {
                double chargeToFullAbsorption = initPower * DamageToChargeFactor;

                if (Charge >= chargeToFullAbsorption)
                {
                    Charge -= chargeToFullAbsorption;

                    return 0;
                }
                else
                {
                    double absorbedDamage = Charge / DamageToChargeFactor;

                    Charge = 0.0;

                    return initPower - absorbedDamage;
                }
            }

            return initPower;
        }
    }
}