﻿using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor
{
    public abstract class EntityMapPosition : MonoBehaviour
    {
        public abstract GridSpace GetType(int teamId);
    }
}