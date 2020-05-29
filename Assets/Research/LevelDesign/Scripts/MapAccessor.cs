using System.Collections;
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
        private static int _mapIntCounter = 2000;

        public int mapId;
        
        public NuclearThroneLevelGenerator generator;

        public DataLogger dataLogger;
        
        private GridSpace[,] _map;

        private void Start()
        {
            generator.onLevelUpdate += UpdateMapData;
        }

        private void AsyncUpdateMapData()
        {
            while(generator.getSpawnPoints == null) {}
            UpdateMapData();
        }

        public GridSpace[,] Map
        {
            get
            {
                if (_map == null)
                {
                    AsyncUpdateMapData();
                    // NuclearThroneMapGenerator.OutputDebugMap(_map);
                }
                return _map;
            }
        }

        private void UpdateMapData()
        {
            _map = NuclearThroneMapFunctions.GenerateArray(generator.width, generator.height);
            foreach (var layers in generator.MapLayerData)
            {
                UpdateTileMap(layers.TileMap, layers.Condition);
            }

            var index = 0;
            for (var i = 0; i < 2; i++)
            {
                var spawnPoint = generator.getSpawnPoints.PointDict[i];
                var cell = GetPosition(spawnPoint.transform.position);

                _map[cell.x, cell.y] = index == 0 ? GridSpace.Spawn1 : GridSpace.Spawn2;
                index++;
            }

            NuclearThroneMapGenerator.OutputDebugMap(_map);
            mapId = _mapIntCounter;
            _mapIntCounter++;
            OutputMap();
        }
        
        public Vector3Int GetPosition(Vector3 position)
        {
            return generator.tilemapGround.WorldToCell(position);
        }
        
        private void UpdateTileMap(Tilemap tileMap, GridSpace type)
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
                        _map[x, y] = type;
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
                var rowData = new List<string[]>();

                var roomHeight = Map.GetUpperBound(0);
                var roomWidth = Map.GetUpperBound(1);

                for (var i = 0; i < roomHeight; i++)
                {
                    var row = new string [roomWidth];
                    for (var j = 0; j < roomWidth; j++)
                    {
                        row[j] = ((int)Map[i, j]).ToString();
                    }
                    rowData.Add(row);
                }
			
                dataLogger.OutputMap(rowData, mapId);
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
