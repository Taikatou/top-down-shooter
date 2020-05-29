using System.Collections.Generic;
using UnityEngine;

namespace Research.LevelDesign.NuclearThrone.Scripts
{
	public static class ClearInnerWalls
	{
		private static int CheckResults(GridSpace[,] map, ISet<Vector2Int> searchedPositions, int x, int y)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			var counter = 0;
			if (map[x, y] == GridSpace.Wall)
			{
				var position = new Vector2Int(x, y);
				var containsAlready = searchedPositions.Contains(position);
				if (!containsAlready)
				{
					counter++;
					searchedPositions.Add(position);

					for (var i = -1; i <= 1; i++)
					{
						for (var j = -1; j <= 1; j++)
						{
							var newX = x + i;
							var newY = y + j;

							var checkX = newX >= 0 && newX < roomWidth;
							var checkY = newY >= 0 && newY < roomHeight;
							if (checkX && checkY)
							{
								counter += CheckResults(map, searchedPositions, x + i, y + j);
							}
						}
					}
				}
			}
				
			return counter;
		}
		
		public static void RemoveInnerWalls(GridSpace[,] map)
		{
			var searchedPositions = new HashSet<Vector2Int>();
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);

			var found = false;
			//loop though every grid space
			for (var x = 0; x < roomWidth && !found; x++)
			{
				for (var y = 0; y < roomHeight && !found; y++)
				{
					var isWall = map[x, y] == GridSpace.Wall;
					if (isWall)
					{
						CheckResults(map, searchedPositions, x, y);
						found = true;
					}
				}
			}

			for (var x = 0; x < roomWidth; x++)
			{
				for (var y = 0; y < roomHeight; y++)
				{
					var isWall = map[x, y] == GridSpace.Wall;
					if (isWall)
					{
						var contains = searchedPositions.Contains(new Vector2Int(x, y));
						if (!contains)
						{
							map[x, y] = GridSpace.Floor;
						}
					}
				}
			}

			RemoveEmptyMiddle(map);
		}
		
		private static void RemoveEmptyMiddle(GridSpace[,] map)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			var assignList = new List<Vector2Int>();

			var open = false;
			
			for (var y = 1; y < roomHeight-1; y++)
			{
				for (var x = 1; x < roomWidth-1; x++)
				{
					if (!open)
					{
						if (map[x, y] == GridSpace.Floor)
						{
							open = true;
						}
					}
					else
					{
						switch (map[x, y])
						{
							case GridSpace.Empty:
								assignList.Add(new Vector2Int(x, y));
								break;
							case GridSpace.Wall:
								open = false;
								break;
						}
					}
				}
				open = false;
			}

			foreach (var item in assignList)
			{
				map[item.x, item.y] = GridSpace.Floor;
			}
		}
	}
}
