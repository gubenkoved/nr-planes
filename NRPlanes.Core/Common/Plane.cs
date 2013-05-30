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
        private double _health;
        public double Health
        {
            get { return _health; }
            private set
            {
                if (value <= 0.0)
                {
                    _health = 0.0;

                    IsGarbage = true;
                }

                _health = Math.Min(MaximalHealth, value);
            }
        }

        [DataMember]
        private List<PlaneEquipment> _allEquipment;
        public IEnumerable<PlaneEquipment> AllEquipment
        {
            get { return _allEquipment; }
        }
        IEnumerable<Equipment> IHaveEquipment.AllEquipment
        {
            get { return _allEquipment; }
        }

        protected Plane(double mass, double angularMass, ReferenceArea referenceArea, double maxHealth)
            : base(mass, angularMass, referenceArea)
        {
            _allEquipment = new List<PlaneEquipment>();

            MaximalHealth = maxHealth;

            Health = MaximalHealth / 2.0;
        }

        protected void AddEquipment(PlaneEquipment equipment)
        {
            equipment.Id = _allEquipment.Count;
            _allEquipment.Add(equipment);

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

        public void Damage(double power)
        {
            if (power < 0.0)
                throw new ArgumentException("power must be >= 0");

            // now only 0 or 1 shield equipment is allowed
            Shield shield = (Shield)_allEquipment.Where(e => e is Shield).SingleOrDefault();

            if (shield != null)
            {
                Health -= shield.Damage(power);
            }
            else
            {
                Health -= power;
            }
        }
        public void Recover(double power)
        {
            if (power < 0.0)
                throw new ArgumentException("power must be >= 0");

            Health += power;
        }

        public abstract void StartMotion(MotionType motion);
        public abstract void EndMotion(MotionType motion);

        public void ActivateShield()
        {
            foreach (Shield shield in _allEquipment.Where(e => e is Shield))
            {
                shield.TurnOn();
            }
            
        }
        public void DeactivateShield()
        {
            foreach (Shield shield in _allEquipment.Where(e => e is Shield))
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