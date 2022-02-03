using System.Collections.Generic;
using System.Linq;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class GetSpawnProcedural : GetEntityProcedural<MlCheckbox>
    {
        public int Players => 2 * EnvironmentInstance.TeamCount;

        public override List<Vector3Int> GetLocations(GridSpace[,] map, int z, int distance=2, int secondDistance=15)
        {
            var array = base.GetLocations(map, z, distance, secondDistance);
            if(Players == 2)
            {
                if (array.Count >= Players)
                {
                    array = GetMaxDistance(array);
                }
            }
            else
            {
                var rnd= new System.Random();
                array = array.OrderBy(x => rnd.Next()).ToList();
            }

            return array;
        }
        
        private List<Vector3Int> GetMaxDistance(IReadOnlyCollection<Vector3Int> array)
        {
            var output = new List<Vector3Int>();
            var maxDistance = -1f;
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
