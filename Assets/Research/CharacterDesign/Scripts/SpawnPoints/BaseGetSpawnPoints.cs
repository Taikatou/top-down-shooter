using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public class BaseGetSpawnPoints : IGetSpawnPoints
    {
        public List<CheckPoint> points;
        public override List<CheckPoint> Points => points;
    }
}
