using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using SpawnPoints;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public class BaseGetSpawnPoints : IGetSpawnPoints
    {
        public List<MLCheckbox> points;
        public override List<MLCheckbox> Points => points;
    }
}
