using System.Collections;
using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Research.LevelDesign.NuclearThrone
{
    public struct MapLayer
    {
        public Tilemap TileMap;
        public TileBase Tile;
        public GridSpace Condition;
    }
    public static class NuclearThroneMapFunctions
    {
        public static void RenderMapWithOffset(GridSpace[,] map, IEnumerable<MapLayer> mapLayerData)
        {
            var roomWidth = map.GetUpperBound(0);
            var roomHeight = map.GetUpperBound(1);
            foreach (var layer in mapLayerData)
            {
                RenderTileMapLayer(map, layer, roomWidth, roomHeight);
            }
        }

        public static void RenderTileMapLayer(GridSpace[,] map, MapLayer layer, int roomWidth, int roomHeight)
        {
            var z = (int) layer.TileMap.transform.position.z;
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    if (map[x, y] == layer.Condition)
                    {
                        var tilePosition = new Vector3Int(x, y, z);
                        layer.TileMap.SetTile(tilePosition, layer.Tile);
                    }
                }
            }   
        }

        public static void ClearArray(GridSpace [,] map, GridSpace clearValue=GridSpace.Empty)
        {
            var roomWidth = map.GetUpperBound(0);
            var roomHeight = map.GetUpperBound(1);
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    map[x, y] = clearValue;
                }
            }
        }

        public static GridSpace[,] GenerateArray(int width, int height)
        {
            var map = new GridSpace[width, height];
            ClearArray(map);
            return map;
        }
    }
}
