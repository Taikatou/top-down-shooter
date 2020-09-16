using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.LevelDesign.NuclearThrone;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Research.LevelDesign.Scripts
{
    public class MapAccessor : MonoBehaviour
    {
        public NuclearThroneLevelGenerator generator;

        private GridSpace[,] _cachedMap;

        private int _cachedMapId = -1;

        private void Start()
        {
            generator.onLevelUpdate += UpdateMapData;
        }

        public GridSpace[,] GetMap(bool overrideCache=false)
        {
            UpdateMapData(overrideCache);
            return _cachedMap;
        }

        public Vector3Int GetPosition(Vector3 position)
        {
            return generator.GetPosition(position);
        }

        private void UpdateMapData()
        {
            UpdateMapData(false);
        }

        private void UpdateMapData(bool overrideCache)
        {
            if (_cachedMapId != generator.mapSeed || overrideCache)
            {
                _cachedMap = NuclearThroneMapFunctions.GenerateArray(generator.width, generator.height);
                foreach (var layers in generator.MapLayerData)
                {
                    UpdateTileMap(_cachedMap, layers.TileMap, layers.Condition);
                }

                var index = 0;
                foreach(var spawnPoint in generator.getSpawnPoints.Points)
                {
                    var cell = generator.GetPosition(spawnPoint.transform.position);

                    _cachedMap[cell.x, cell.y] = index == 0 ? GridSpace.Spawn1 : GridSpace.Spawn2;
                    index++;
                }
                foreach (var point in generator.pickupProcedural.Points)
                {
                    var cell = generator.GetPosition(point.transform.position);
                    _cachedMap[cell.x, cell.y] = GridSpace.Health;
                }

                //NuclearThroneMapGenerator.OutputDebugMap(map);
                _cachedMapId = generator.mapSeed;
            }
        }

        private void UpdateTileMap(GridSpace[,] map, Tilemap tileMap, GridSpace type)
        {
            var z = (int) tileMap.transform.position.y;

            var found = false;

            for (var y = 0; y < generator.height; y++)
            {
                for (var x = 0; x < generator.width; x++)
                {
                    var tilePosition = new Vector3Int(x, y, z);
                    var tile = tileMap.GetTile(tilePosition);
                    if (tile != null)
                    {
                        map[x, y] = type;
                        if (!found)
                        {
                            found = true;
                        }
                    }
                }
            }

            if (!found)
            {
                Debug.Log("did not find any tiles");
            }
        }

        public void OutputMap()
        {
            var map = GetMap();
            OutputMap(map);
        }

        private void OutputMap(GridSpace[,] map)
        {
            //dataLogger.OutputMap(map, _cachedMapId);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MapAccessor))]
    public class MapAccessorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Reference to our script
            var levelGen = (MapAccessor) target;

            if (GUILayout.Button("Output map"))
            {
                levelGen.OutputMap();
            }
        }
    }
#endif
}
