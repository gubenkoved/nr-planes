﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using System.Runtime.Serialization;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Core.Bonuses
{
    [DataContract]
    [KnownType(typeof(HealthBonus))]
    [KnownType(typeof(RandomBonus))]
    public abstract class Bonus : GameObject
    {
        [DataMember]
        private double m_health;
        public double Health
        {
            get
            {
                return m_health;
            }
        }

        public Bonus(Vector position, double health = 100)
            :base(1, 1, null)
        {
            m_health = 100;

            Position = position;
            RelativeGeometry = new CircleGeometry(Vector.Zero, 5);
        }

        public void Damage(double power)
        {
            m_health = Math.Max(0, m_health - power);

            if (m_health == 0)
                IsGarbage = true;
        }

        /// <summary>
        /// Applies some bonus effects to plane
        /// </summary>
        public void Apply(Plane plane)
        {
            IsGarbage = true;

            ApplyImpl(plane);
        }

        protected abstract void ApplyImpl(Plane plane);
    }
}