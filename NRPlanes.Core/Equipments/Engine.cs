﻿using System;
using NRPlanes.Core.Primitives;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Equipments
{
    [DataContract]
    [KnownType(typeof(Engines.IonEngine))]
    [KnownType(typeof(Engines.RocketEngine))]
    public abstract class Engine : PlaneEquipment
    {
        [DataMember]
        public bool IsActive { get; private set; }

        [DataMember]
        public double TractionForce { get; private set; }

        protected Engine(double tractionForce, double maxCharge, double regeneration)
            : base(maxCharge, regeneration)
        {
            TractionForce = tractionForce;
            MaximumCharge = maxCharge;
            Regeneration = regeneration;
        }

        public void TurnOn()
        {
            IsActive = true;
        }

        public void TurnOff()
        {
            IsActive = false;
        }

        public override void Update(TimeSpan elapsed)
        {
            base.Update(elapsed);

            if (IsActive && Charge > elapsed.TotalSeconds)
            {
                double absoluteRotation = RelatedGameObject.GetEquipmentAbsoluteRotation(this);
                Vector absolutePosition = RelatedGameObject.GetEquipmentAbsolutePosition(this);

                Vector tractionForce = new Vector(0, TractionForce).Rotate(absoluteRotation);

                RelatedGameObject.Affect(tractionForce, absolutePosition);

                Charge -= elapsed.TotalSeconds;
            }
        }
    }
}