using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using SpawnPoints;

namespace Research.LevelDesign.NuclearThrone
{
    public class CheckpointSpawner : IGetSpawnPoints
    {
        public override List<MLCheckbox> Points { get; }
    }
}
