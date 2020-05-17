using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.Common.MapSensor.Sensor;
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

        private GridSpace[,] _map;

        public DataLogger dataLogger;

        private void Start()
        {
            generator.onLevelUpdate += UpdateMapData;
        }

        public GridSpace[,] Map
        {
            get
            {
                if (_map == null)
                {
                    UpdateMapData();
                    TileMapSensor.OutputDebugMap(_map);
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

            OutputMap();
        }
        
        public Vector3Int GetPosition(Vector3 position)
        {
            return generator.tilemapGround.WorldToCell(position);
        }
        
        private void UpdateTileMap(Tilemap tileMap, GridSpace type)
        {
            var z = (int) tileMap.transform.position.y;

            for (var y = 0; y < generator.height; y++)
            {
                for (var x = 0; x < generator.width; x++)
                {
                    var tilePosition = new Vector3Int(x, y, z);
                    var tile = tileMap.GetTile(tilePosition);
                    if (tile != null)
                    {
                        _map[x, y] = type;
                    }
                    else
                    {
                        Debug.Log("Invalid Tilemap");
                    }
                }
            }
        }
        
        public void OutputMap()
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
			
            dataLogger.OutputMap(rowData);
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
