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
            
            for (var i = 0; i < roomHeight; i++)
            {
                for (var j = 0; j < roomWidth; j++)
                {
                    var tilePosition = new Vector3Int(j, i, z);
                    if(map[i,j] == GridSpace.Wall)
                    {
                        tilemapGround.SetTile(tilePosition, tileWalls);
                    }
                    if(map[i,j] == GridSpace.Floor)
                    {
                        tilemapWall.SetTile(tilePosition, tileGround);
                    }
                }
            }
        }

        public static void ClearArray(GridSpace [,] map, bool empty)
        {
            var roomWidth = map.GetUpperBound(0);
            var roomHeight = map.GetUpperBound(1);
            for (var i = 0; i < roomHeight; i++)
            {
                for (var j = 0; j < roomWidth; j++)
                {
                    if (empty)
                    {
                        map[i, j] = GridSpace.Empty;
                    }
                    else
                    {
                        map[i, j] = GridSpace.Floor;
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
