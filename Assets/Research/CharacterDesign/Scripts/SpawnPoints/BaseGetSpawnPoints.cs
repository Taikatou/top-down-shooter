using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;

namespace SpawnPoints
{
    public class BaseGetSpawnPoints : IGetSpawnPoints
    {
        public List<MLCheckbox> points;
        public override List<MLCheckbox> Points => points;
    }
}
