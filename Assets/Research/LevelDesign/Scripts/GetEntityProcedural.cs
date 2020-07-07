using System.Collections.Generic;
using Research.CharacterDesign.Scripts.SpawnPoints;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public abstract class GetEntityProcedural<T> : GetSpawnPoints<T> where T : MonoBehaviour, IEntityClass
    {
        public GameObject entityPrefab;

        private bool set;
        
        private Dictionary<int, T> _pointDict;

        public override T[] Points => GetComponentsInChildren<T>();

        private void Start()
        {
            SetPoints();
        }

        public Dictionary<int, T> PointDict
        {
            get
            {
                SetPoints();

                return _pointDict;
            }
        }

        private void SetPoints()
        {
            if (!set)
            {
                _pointDict = new Dictionary<int, T>();
                set = true;
                foreach (var point in Points)
                {
                    AddPoint(point.GetId(), point);
                }
            }
        }

        public void AddPoint(int playerId, T checkpoint)
        {
            if (Application.isPlaying)
            {
                var contains = PointDict.ContainsKey(playerId);
                if (!contains)
                {
                    PointDict.Add(playerId, checkpoint);
                }
                else
                {
                    PointDict[playerId] = checkpoint;
                }   
            }
        }

        private bool FreeTile(int x, int y, GridSpace[,] map, int distance)
        {
            for (var i = -distance; i <= distance; i++)
            {
                for (var j = -distance; j <= distance; j++)
                {
                    if (map[x + i, y + j] != GridSpace.Floor)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        public virtual List<Vector3Int> GetLocations(GridSpace[,] map, int z, int freeDistance = 2, int minDistance=15)
        {
            var validPositions = new List<Vector3Int>();
            var width = map.GetUpperBound(0);
            var height = map.GetUpperBound(1);
            for (var y = freeDistance; y < height - freeDistance; y++)
            {
                for (var x = freeDistance; x < width - freeDistance; x++)
                {
                    var free = FreeTile(x, y, map, freeDistance);
                    var notClose = CheckDistance(x, y, validPositions, minDistance);
                    if (free && notClose)
                    {
                        validPositions.Add(new Vector3Int(x, y, z));
                    }
                }
            }

            return validPositions;
        }

        private bool CheckDistance(int x, int y, IEnumerable<Vector3Int> availableSpots, int distance)
        {
            foreach (var spot in availableSpots)
            {
                var xDistance = Mathf.Abs(spot.x - x);
                var yDistance = Mathf.Abs(spot.y - y);
                if (xDistance + yDistance < distance)
                {
                    return false;
                }
            }

            return true;
        }

        public void DestroyEntity()
        {
            foreach (var point in Points)
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(point.gameObject);
                }
                else
                {
                    Destroy(point.gameObject);
                }
            }
        }
    }
}
