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

        public DataLogger dataLogger;

        private GridSpace[,] _cachedMap;

        private int _cachedMapId = -1;

        private void Start()
        {
            generator.onLevelUpdate += UpdateMapData;
        }

        public GridSpace[,] GetMap()
        {
            UpdateMapData();
            return _cachedMap;
        }

        private void UpdateMapData()
        {
            var newId = generator.instanceMapCounter;
            if (_cachedMapId != newId)
            {
                _cachedMap = NuclearThroneMapFunctions.GenerateArray(generator.width, generator.height);
                foreach (var layers in generator.MapLayerData)
                {
                    UpdateTileMap(_cachedMap, layers.TileMap, layers.Condition);
                }

                var index = 0;
                for (var i = 0; i < 2; i++)
                {
                    var spawnPoint = generator.getSpawnPoints.PointDict[i];
                    var cell = GetPosition(spawnPoint.transform.position);

                    _cachedMap[cell.x, cell.y] = index == 0 ? GridSpace.Spawn1 : GridSpace.Spawn2;
                    index++;
                }

                foreach (var point in generator.pickupProcedural.Points)
                {
                    var cell = GetPosition(point.transform.position);
                    _cachedMap[cell.x, cell.y] = GridSpace.Health;
                }

                //NuclearThroneMapGenerator.OutputDebugMap(map);
                _cachedMapId = newId;
            }
        }
        
        public Vector3Int GetPosition(Vector3 position)
        {
            return generator.tilemapGround.WorldToCell(position);
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
            if (dataLogger.outputCsv)
            {
                var map = GetMap();
                var rowData = new List<string[]>();

                var roomHeight = map.GetUpperBound(0);
                var roomWidth = map.GetUpperBound(1);

                for (var i = 0; i < roomHeight; i++)
                {
                    var row = new string [roomWidth];
                    for (var j = 0; j < roomWidth; j++)
                    {
                        row[j] = ((int)map[i, j]).ToString();
                    }
                    rowData.Add(row);
                }
			
                dataLogger.OutputMap(rowData, _cachedMapId);
            }
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
