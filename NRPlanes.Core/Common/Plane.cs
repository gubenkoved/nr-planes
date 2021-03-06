﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NRPlanes.Core.Equipments;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Common
{
    [DataContract]
    [KnownType(typeof(Planes.XWingPlane))]
    public abstract class Plane : GameObject, IHaveEquipment<PlaneEquipment>
    {
        [DataMember]
        public Guid PlayerGuid { get; set; }

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
        private Dictionary<PlaneEquipmentRelativeInfo, PlaneEquipment> m_allEquipment;
        public IDictionary<PlaneEquipmentRelativeInfo, PlaneEquipment> AllEquipment
        {
            get { return m_allEquipment; }
        }
        IEnumerable<PlaneEquipment> IHaveEquipment<PlaneEquipment>.AllEquipment
        {
            get { return m_allEquipment.Values; }
        }
        IEnumerable<Equipment> IHaveEquipment.AllEquipment
        {
            get { return m_allEquipment.Values; }
        }

        protected Plane(double mass, double angularMass, Geometry relativeGeometry, ReferenceArea referenceArea, double maxHealth)
            : base(mass, angularMass, relativeGeometry, referenceArea)
        {
            m_allEquipment = new Dictionary<PlaneEquipmentRelativeInfo, PlaneEquipment>();

            MaximalHealth = maxHealth;
            Health = MaximalHealth / 2.0;
        }

        private void AddEquipment(PlaneEquipment equipment, PlaneEquipmentRelativeInfo relInfo, bool supressRaisingEquipmentAddedEvent)
        {
            equipment.RelatedGameObject = this;
            equipment.Id = m_allEquipment.Count;

            m_allEquipment.Add(relInfo, equipment);

            if (!supressRaisingEquipmentAddedEvent)
            {
                OnEquipmentStatusChanged(this,
                    new GameObjectEquipmentStatusChangedArgs(
                        GameObjectEquipmentStatus.Added,
                        equipment));
            }
        }
        protected void AddEquipmentSuppressEvents(PlaneEquipment equipment, PlaneEquipmentRelativeInfo relInfo)
        {
            AddEquipment(equipment, relInfo, true);
        }
        public void AddEquipment(PlaneEquipment equipment, PlaneEquipmentRelativeInfo relInfo)
        {
            AddEquipment(equipment, relInfo, false);
        }
        public void RemoveEquipment(PlaneEquipment equipment)
        {
            if (m_allEquipment.ContainsValue(equipment))
            {
                var key = GetEquipmentRelativeInfo(equipment);
                
                m_allEquipment.Remove(key);

                OnEquipmentStatusChanged(this,
                    new GameObjectEquipmentStatusChangedArgs(
                        GameObjectEquipmentStatus.Removed,
                        equipment));
            }
            else
            {
                throw new Exception("There is no this equipment");
            }
        }
        public PlaneEquipment GetEquipmentById(int id)
        {
            return m_allEquipment.Single(kvp => kvp.Value.Id == id).Value;
        }

        public PlaneEquipmentRelativeInfo GetEquipmentRelativeInfo(PlaneEquipment planeEquipment)
        {
            return m_allEquipment.Single(kvp => kvp.Value == planeEquipment).Key;
        }
        public Vector GetEquipmentAbsolutePosition(PlaneEquipment planeEquipment)
        {
            PlaneEquipmentRelativeInfo equipmentInfo = GetEquipmentRelativeInfo(planeEquipment);

            return Position + equipmentInfo.RelativeToOriginPosition.Rotate(Rotation);
        }
        public double GetEquipmentAbsoluteRotation(PlaneEquipment planeEquipment)
        {
            PlaneEquipmentRelativeInfo equipmentInfo = GetEquipmentRelativeInfo(planeEquipment);

            return Rotation + equipmentInfo.RelativeRotation;
        }

        public void Fire(WeaponPosition weaponPosition = WeaponPosition.Unknown)
        {
            if (weaponPosition == WeaponPosition.Unknown)
            {
                foreach (var weapon in m_allEquipment.Values.Where(equip => equip is Weapon).Cast<Weapon>())
                {
                    weapon.Fire();
                }
            }
            else
            {
                foreach (var infoWithWeapon in m_allEquipment.Where(infoPlusWeaponPair => infoPlusWeaponPair.Key is PlaneWeaponRelativeInfo))
                {
                    PlaneWeaponRelativeInfo weaponRelInfo = (PlaneWeaponRelativeInfo)infoWithWeapon.Key;
                    Weapon weapon = (Weapon)infoWithWeapon.Value;

                    if (weaponRelInfo.WeaponPosition == weaponPosition)
                        weapon.Fire();
                }
            }
        }

        public void Damage(double damage)
        {
            if (damage < 0.0)
                throw new ArgumentException("power must be >= 0");

            // now only 0 or 1 shield equipment is allowed
            Shield shield = (Shield)m_allEquipment.Values.SingleOrDefault(e => e is Shield);

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

        public abstract void StartMotion(PlaneMotionType motion);
        public abstract void EndMotion(PlaneMotionType motion);

        public void ActivateShield()
        {
            foreach (Shield shield in m_allEquipment.Values.Where(e => e is Shield))
            {
                shield.TurnOn();
            }
            
        }
        public void DeactivateShield()
        {
            foreach (Shield shield in m_allEquipment.Values.Where(e => e is Shield))
            {
                shield.TurnOff();
            }
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            foreach (var planeEquipment in m_allEquipment.Values)
            {
                planeEquipment.Update(elapsed);
            }
        }

        private void OnEquipmentStatusChanged(object sender, GameObjectEquipmentStatusChangedArgs args)
        {
            if (EquipmentStatusChanged != null)
            {
                EquipmentStatusChanged.Invoke(sender, args);
            }
        }
        public event EventHandler<GameObjectEquipmentStatusChangedArgs> EquipmentStatusChanged;
    }
}