using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;

namespace Research.CharacterDesign.Scripts.SpawnPoints
{
    public class BaseGetSpawnPoints : GetSpawnPoints<MlCheckbox>
    {
        public List<MlCheckbox> points;
        public override MlCheckbox[] Points => points.ToArray();
    }
}
