using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class GetPickupProcedural : GetEntityProcedural<PickableItem>
    {
        public int minDistance = 15;
        public int healthPositionCount = 3;
        public int startDistance = 12;
        public List<Vector3Int> GetHealthPositions(GridSpace[,] map, List<Vector3Int> avoidPositions, int z, int freeDistance=2)
        {
            var newLocations = new List<Vector3Int>();
            if (healthPositionCount > 0)
            {
                var newStartDistance = startDistance;
                var locations = GetLocations(map, z, freeDistance, minDistance);
                foreach (var location in locations)
                {
                    var valid = true;
                    foreach (var toAvoid in avoidPositions)
                    {
                        var entityDistance = EntityUtils.GetDistance(location, toAvoid);
                        if (entityDistance < newStartDistance)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        newLocations.Add(location);
                        avoidPositions.Add(location);
                        if (newLocations.Count >= healthPositionCount)
                        {
                            break;
                        }
                    }
                }
            }
            return newLocations;
        }
    }
}
