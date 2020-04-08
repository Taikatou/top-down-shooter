using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Research.LevelDesign.NuclearThrone
{
    public static class NuclearThroneMapFunctions
    {
        public static void RenderMapWithOffset(GridSpace[,] map, Tilemap tilemapGround, Tilemap tilemapWall,
            TileBase tileWalls, TileBase tileGround, Vector2Int offset = new Vector2Int())
        {
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                for (var y = 0; y < map.GetUpperBound(1); y++)
                {
                    if(map[x,y] == GridSpace.Wall)
                    {
                        RenderTile(x, y, tilemapWall, tileWalls, offset);
                    }
                    if(map[x,y] == GridSpace.Floor)
                    {
                        RenderTile(x, y, tilemapGround, tileGround, offset);
                    }
                }
            }
        }

        private static void RenderTile(int x, int y, Tilemap tilemap, TileBase tile, Vector2Int offset = new Vector2Int())
        {
            tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y ,0), tile);
        }

        private static void RenderTileX4(int x, int y, Tilemap tilemap, TileBase tile,
            Vector2Int offset = new Vector2Int())
        {
            var newX = x * 2;
            var newY = y * 2;
            RenderTile(newX, newY, tilemap, tile, offset);
            RenderTile(newX + 1, newY, tilemap, tile, offset);
            RenderTile(newX, newY + 1, tilemap, tile, offset);
            RenderTile(newX + 1, newY + 1, tilemap, tile, offset);
        }

        public static GridSpace[,] GenerateArray(int width, int height, bool empty)
        {
            var map = new GridSpace[width, height];
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
            return map;
        }
    }
}
