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
                    if(map[x,y] == GridSpace.Wall)
                    {
                        tilemapGround.SetTile(tilePosition, tileWalls);
                    }
                    if(map[x,y] == GridSpace.Floor)
                    {
                        tilemapWall.SetTile(tilePosition, tileGround);
                    }
                }
            }
        }

        public static void ClearArray(GridSpace [,] map, bool empty)
        {
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                for (var y = 0; y < map.GetUpperBound(1); y++)
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
