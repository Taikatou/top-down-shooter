using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.SpawnPoints;

namespace Research.LevelDesign.NuclearThrone
{
    public class CheckpointSpawner : IGetSpawnPoints
    {
        public override List<CheckPoint> Points { get; }
    }
}
