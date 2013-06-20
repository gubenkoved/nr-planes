﻿using System;
using System.Collections.Generic;

namespace NRPlanes.Core.Equipments
{
    public interface IHaveEquipment
    {
        IEnumerable<Equipment> AllEquipment { get; }
    }

    public interface IHaveEquipment<out T> : IHaveEquipment where T : Equipment
    {
        new IEnumerable<T> AllEquipment { get; }
    }
}