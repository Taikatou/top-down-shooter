using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class GetSpawnProcedural : GetEntityProcedural<MlCheckbox>
    {
        public int players = 2;

        public override List<Vector3Int> GetLocations(GridSpace[,] map, int z, int distance=2, int secondDistance=15)
        {
            var array = base.GetLocations(map, z, distance, secondDistance);
            if (array.Count >= players)
            {
                return GetMaxDistance(array);
            }

            return array;
        }
        
        private List<Vector3Int> GetMaxDistance(IReadOnlyCollection<Vector3Int> array)
        {
            var output = new List<Vector3Int>();
            float maxDistance = -1f;
            foreach (var item in array)
            {
                foreach (var secondItem in array)
                {
                    if (item != secondItem)
                    {
                        var distance = EntityUtils.GetDistance(item, secondItem);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            output = new List<Vector3Int> {item, secondItem};
                        }
                    }
                }
            }
            return output;
        }
    }
}
