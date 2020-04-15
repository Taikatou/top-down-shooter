using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public abstract class IGetSpawnPoints : MonoBehaviour
    {
        public abstract List<MLCheckbox> Points { get; }
    }
}
