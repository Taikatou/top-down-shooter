using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Research.LevelDesign.NuclearThrone
{
    public static class NuclearThroneMapFunctions
    {
        public static void RenderMapWithOffset(GridSpace[,] map, Tilemap tilemapGround, Tilemap tilemapWall,
            TileBase tileWalls, TileBase tileGround)
        {
            var roomWidth = map.GetUpperBound(0);
            var roomHeight = map.GetUpperBound(1);
            var z = (int) tilemapGround.transform.position.y;
            
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    var tilePosition = new Vector3Int(x, y, z);
                    switch (map[x, y])
                    {
                        case GridSpace.Wall:
                            tilemapGround.SetTile(tilePosition, tileWalls);
                            break;
                        case GridSpace.Floor:
                            tilemapWall.SetTile(tilePosition, tileGround);
                            break;
                    }
                }
            }
        }

        public static void ClearArray(GridSpace [,] map, bool empty)
        {
            var roomWidth = map.GetUpperBound(0);
            var roomHeight = map.GetUpperBound(1);
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    if (empty)
                    {
                        map[x, y] = GridSpace.Empty;
                    }
                    else
                    {
                        map[x, y] = GridSpace.Floor;
                    }
                }
            }
        }

        public static GridSpace[,] GenerateArray(int width, int height)
        {
            var map = new GridSpace[width, height];
            return map;
        }
    }
}
