using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using SpawnPoints;

namespace Research.LevelDesign.Scripts
{
    public class GetSpawnProcedural : IGetSpawnPoints
    {
        public override List<MLCheckbox> Points => new List<MLCheckbox>(GetComponentsInChildren<MLCheckbox>());
    }
}
