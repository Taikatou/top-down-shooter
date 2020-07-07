using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.CharacterDesign.Scripts.SpawnPoints;

namespace Research.LevelDesign.NuclearThrone
{
    public class CheckpointSpawner : GetSpawnPoints<MlCheckbox>
    {
        public override MlCheckbox[] Points { get; }
    }
}
