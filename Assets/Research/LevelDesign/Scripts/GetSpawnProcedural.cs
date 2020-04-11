using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using Research.CharacterDesign.Scripts.SpawnPoints;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class GetSpawnProcedural : IGetSpawnPoints
    {

        public override List<CheckPoint> Points => new List<CheckPoint>(GetComponentsInChildren<MLCheckbox>());
    }
}
