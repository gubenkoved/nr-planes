using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Common
{
    public enum MotionType
    {
        Forward,
        Left,
        Right,
        All
    }

    [DataContract]
    [KnownType(typeof(Planes.XWingPlane))]
    public abstract class Plane : GameObject, IHaveEquipment<PlaneEquipment>
    {
        //public Guid PlayerGuid { get; private set; }
        [DataMember]
        public double MaximalHealth { get; protected set; }

        [DataMember]
        private double m_health;
        public double Health
        {
            get { return m_health; }
            private set
            {
                m_health = value;

                if (value <= 0.0)
                {
                    m_health = 0.0;

                    IsGarbage = true;
                }

                m_health = Math.Min(MaximalHealth, value);
            }
        }

        [DataMember]
        private List<PlaneEquipment> m_allEquipment;
        public IEnumerable<PlaneEquipment> AllEquipment
        {
            get { return m_allEquipment; }
        }
        IEnumerable<Equipment> IHaveEquipment.AllEquipment
        {
            get { return m_allEquipment; }
        }

        protected Plane(double mass, double angularMass, ReferenceArea referenceArea, double maxHealth)
            : base(mass, angularMass, referenceArea)
        {
            m_allEquipment = new List<PlaneEquipment>();

            MaximalHealth = maxHealth;

            Health = MaximalHealth / 2.0;
        }

        protected void AddEquipment(PlaneEquipment equipment)
        {
            equipment.Id = m_allEquipment.Count;
            m_allEquipment.Add(equipment);

            equipment.RelatedGameObject = this;
        }

        public void Fire(WeaponPosition weaponPosition = WeaponPosition.Unknown)
        {
            if (weaponPosition == WeaponPosition.Unknown)
            {
                foreach (var weapon in AllEquipment.OfType<Weapon>())
                {
                    weapon.Fire();
                }
            }
            else
            {
                foreach (var weapon in AllEquipment.OfType<Weapon>().Where(w => w.OnPlanePosition == weaponPosition))
                {
                    weapon.Fire();
                }
            }
        }

        public void Damage(double damage)
        {
            if (damage < 0.0)
                throw new ArgumentException("power must be >= 0");

            // now only 0 or 1 shield equipment is allowed
            Shield shield = (Shield)m_allEquipment.Where(e => e is Shield).SingleOrDefault();

            double effectiveDamage = damage;

            if (shield != null)
                effectiveDamage = shield.CalculateEffectiveDamage(damage);

            m_health = Math.Max(0, m_health - effectiveDamage);

            if (m_health == 0)
            {
                IsGarbage = true;
            }
        }
        public void Recover(double healthDelta)
        {
            if (healthDelta < 0.0)
                throw new ArgumentException("power must be >= 0");

            m_health = Math.Min(MaximalHealth, m_health + healthDelta);
        }

        public abstract void StartMotion(MotionType motion);
        public abstract void EndMotion(MotionType motion);

        public void ActivateShield()
        {
            foreach (Shield shield in m_allEquipment.Where(e => e is Shield))
            {
                shield.TurnOn();
            }
            
        }
        public void DeactivateShield()
        {
            foreach (Shield shield in m_allEquipment.Where(e => e is Shield))
            {
                shield.TurnOff();
            }
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            foreach (var planeEquipment in AllEquipment)
            {
                planeEquipment.Update(elapsed);
            }
        }
    }
}