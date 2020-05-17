using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public class BaseGetSpawnPoints : IGetSpawnPoints
    {
        public List<MLCheckbox> points;
        public override MLCheckbox[] Points => points.ToArray();
    }
}
