using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public abstract class IGetSpawnPoints : MonoBehaviour
    {
        public abstract List<CheckPoint> Points { get; }
    }
}
