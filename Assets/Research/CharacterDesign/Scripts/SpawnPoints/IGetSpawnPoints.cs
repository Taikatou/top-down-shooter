﻿using System.Collections.Generic;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public abstract class IGetSpawnPoints<T> : MonoBehaviour where T : MonoBehaviour
    {
        public abstract T[] Points { get; }
    }
}
