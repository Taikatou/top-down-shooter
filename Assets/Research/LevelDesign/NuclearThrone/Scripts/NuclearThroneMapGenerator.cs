using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Research.LevelDesign.NuclearThrone.Scripts
{
	public enum GridSpace {Empty, Wall, Floor, Team1, Team2, Coin, Projectile1, Projectile2, Spawn1, Spawn2, Health}
	public static class NuclearThroneMapGenerator
	{
		private struct Walker
		{
			public Vector2Int Dir;
			public Vector2Int Pos;
		}
		
		public static GridSpace[,] GenerateMap (GridSpace[,] map, float percentToFill = 0.2f)
		{
			CreateFloors(map, percentToFill);
			
			CreateWalls(map);
			
			RemoveSingleWalls(map, percentToFill);

			return map;
		}

		public static GridSpace[,] SquareMap(GridSpace[,] map, int border = 7)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			for (var x = 0; x < roomWidth; x++)
			{
				for (var y = 0; y < roomHeight; y++)
				{
					var checkX = x < border || x + border >= roomWidth; 
					var checkY = y < border || y + border >= roomHeight;

					map[x, y] = (checkX || checkY) ? GridSpace.Wall : GridSpace.Floor;
				}
			}

			return map;
		}
		
		public static void OutputDebugMap(GridSpace [,] debugGrid)
		{
			var roomWidth = debugGrid.GetUpperBound(0);
			var roomHeight = debugGrid.GetUpperBound(1);
			var output = "Output log \n";
			for (var y = roomHeight - 1; y >= 0; y--)
			{
				for (var x = 0; x < roomWidth; x++)
				{
					output += (int)debugGrid[x, y] + ",";
				}
				output += "\n";
			}
			Debug.Log(output);
		}

		private static void CreateWalls(GridSpace[,] map)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);

			void WallCheck(int xL, int yL)
			{
				if (map[xL, yL] == GridSpace.Empty)
				{
					map[xL, yL] = GridSpace.Wall;
				}
			}

			//loop though every grid space
			for (var x = 0; x < roomWidth-1; x++)
			{
				for (var y = 0; y < roomHeight-1; y++)
				{
					//if theres a floor, check the spaces around it
					if (map[x, y] == GridSpace.Floor)
					{
						//if any surrounding spaces are empty, place a wall
						WallCheck(x, y + 1);
						WallCheck(x, y -1);
						WallCheck(x + 1, y);
						WallCheck(x - 1, y);
					}
				}
			}
		}

		private static void CreateFloors(GridSpace[,] map, float percentToFill)
		{
			const float chanceWalkerChangeDir = 0.5f;
			const float chanceWalkerSpawn = 0.18f;
			const float chanceWalkerDestroy = 0.10f;
			const float maxWalkers = 20;

			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			//find center of grid
			var spawnPos = new Vector2Int(Mathf.RoundToInt(roomWidth/ 2.0f),
										Mathf.RoundToInt(roomHeight/ 2.0f));
			
			var newWalker = new Walker {Dir = RandomDirection(), Pos = spawnPos};
			//add walker to list
			var walkers = new List<Walker> { newWalker };

			var iterations = 0;
			//loop will not run forever
			do
			{
				//create floor at position of every walker
				foreach (var myWalker in walkers)
				{
					map[myWalker.Pos.x, myWalker.Pos.y] = GridSpace.Floor;
				}
				//chance: destroy walker
				var numberChecks = walkers.Count; //might modify count while in this loop
				for (var i = 0; i < numberChecks; i++)
				{
					//only if its not the only one, and at a low chance
					if (Random.value < chanceWalkerDestroy && walkers.Count > 1)
					{
						walkers.RemoveAt(i);
						break; //only destroy one per iteration
					}
				}
				//chance: walker pick new direction
				for (var i = 0; i < walkers.Count; i++)
				{
					if (Random.value < chanceWalkerChangeDir)
					{
						var thisWalker = walkers[i];
						thisWalker.Dir = RandomDirection();
						walkers[i] = thisWalker;
					}
				}
				//chance: spawn new walker
				numberChecks = walkers.Count; //might modify count while in this loop
				for (var i = 0; i < numberChecks; i++)
				{
					//only if # of walkers < max, and at a low chance
					if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers)
					{
						//create a walker 
						var tmpWalker = new Walker {Dir = RandomDirection(), Pos = walkers[i].Pos};
						walkers.Add(tmpWalker);
					}
				}
				//move walkers
				for (var i = 0; i < walkers.Count; i++)
				{
					var thisWalker = walkers[i];
					thisWalker.Pos += thisWalker.Dir;
					walkers[i] = thisWalker;				
				}
				//avoid boarder of grid
				for (var i =0; i < walkers.Count; i++)
				{
					var thisWalker = walkers[i];
					//clamp x,y to leave a 1 space boarder: leave room for walls
					thisWalker.Pos.x = Mathf.Clamp(thisWalker.Pos.x, 1, roomWidth-2);
					thisWalker.Pos.y = Mathf.Clamp(thisWalker.Pos.y, 1, roomHeight-2);
					walkers[i] = thisWalker;
				}
				//check to exit loop
				var exitCondition = NumberOfFloors(map) / (float) map.Length > percentToFill;
				if (exitCondition)
				{
					break;
				}
				iterations++;
			}
			while(iterations < 100000);
		}


		private static void RemoveSingleWalls(GridSpace[,] map, float requiredFloorPercent)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			//loop though every grid space
			for (var x = 0; x < roomWidth-1; x++)
			{
				for (var y = 0; y < roomHeight-1; y++)
				{
					//if theres a wall, check the spaces around it
					if (map[x,y] == GridSpace.Wall)
					{
						//assume all space around wall are floors
						var allFloors = true;
						//check each side to see if they are all floors
						for (var checkX = -1; checkX <= 1 ; checkX++)
						{
							for (var checkY = -1; checkY <= 1; checkY++)
							{
								if (x + checkX < 0 || x + checkX > roomWidth - 1 || 
								    y + checkY < 0 || y + checkY > roomHeight - 1)
								{
									//skip checks that are out of range
									continue;
								}
								if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0))
								{
									//skip corners and center
									continue;
								}
								if (map[x + checkX,y+checkY] != GridSpace.Floor)
								{
									allFloors = false;
								}
							}
						}
						if (allFloors)
						{
							map[x,y] = GridSpace.Floor;
						}
					}
				}
			}
		}
		
		private static Vector2Int RandomDirection()
		{
			//pick random int between 0 and 3
			var choice = Mathf.FloorToInt(Random.value * 3.99f);
			//use that int to chose a direction
			switch (choice)
			{
				case 0:
					return Vector2Int.down;
				case 1:
					return Vector2Int.left;
				case 2:
					return Vector2Int.up;
				default:
					return Vector2Int.right;
			}
		}

		private static int NumberOfFloors(GridSpace[,] map)
		{
			return map.Cast<GridSpace>().Count(space => space == GridSpace.Floor);
		}
	}
}
