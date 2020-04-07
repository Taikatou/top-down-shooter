using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Research.LevelDesign.NuclearThrone.Scripts
{
	public enum GridSpace {Empty, Wall, Floor}
	public static class NuclearThroneLevelGenerator
	{
		private struct Walker
		{
			public Vector2 Dir;
			public Vector2 Pos;
		}
		
		public static int[,] GenerateMap (int[,] map, float seed, float percentToFill = 0.2f)
		{
			map = CreateFloors(map, seed, percentToFill);
			
			map = CreateWalls(map);

			map = RemoveSingleWalls(map, seed, percentToFill);
			
			map = new int [35, 35] {{0,	2,	2,	2,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	2,	2,	2,	2,	2,	2,	0,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	2,	2,	1,	1,	1,	1,	1,	1,	2,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	2,	0,	0,	2,	2,	1,	1,	1,	1,	1,	1,	1,	1,	2,	1,	2,	2,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	2,	2,	2,	1,	1,	1,	2,	2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	2,	1,	1,	1,	1,	2,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	1,	2,	2,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {0,	2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	1,	2,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {0,	2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	1,	2,	0,	2,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {0,	2,	1,	1,	1,	1,	1,	2,	1,	1,	1,	1,	1,	1,	1,	2,	2,	0,	0,	0,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	2,	1,	1,	1,	1,	1,	2,	2,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	1,	1,	1,	1,	2,	2,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {2,	1,	1,	1,	2,	1,	2,	1,	1,	1,	1,	2,	2,	2,	1,	1,	2,	0,	0,	0,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	2,	1,	1,	2,	1,	2,	1,	1,	1,	1,	1,	2,	0,	2,	1,	2,	0,	0,	0,	0,	0,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	0,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	2,	2,	2,	2,	1,	1,	1,	1,	1,	1,	2,	2,	2,	1,	2,	2,	2,	2,	0,	0,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	2,	1,	1,	2,	2,	2,	1,	1,	1,	2,	1,	1,	1,	1,	1,	2,	2,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	2,	2,	0,	0,	2,	1,	1,	1,	2,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	2,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	2,	1,	2,	2,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	2,	1,	1,	2,	1,	1,	1,	1,	1,	2,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	2,	1,	1,	1,	1,	1,	1,	1,	1,	2,	2,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	2,	2,	1,	1,	2,	2,	1,	1,	1,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	2,	1,	1,	2,	2,	1,	2,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	2,	2,	0,	0,	2,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	},{0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0	}, {0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0,	0}};
			
			map = CleanMap(map);
			
			return map;
		}

		private static int[,] CleanMap(int[,] map)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			for (var x = 0; x < roomWidth - 1; x++)
			{
				for (var y = 0; y < roomHeight - 1; y++)
				{
					if (map[x, y] == 1)
					{
						map[x, y] = 2;
					}
					else if (map[x, y] == 2)
					{
						map[x, y] = 1;
					}
				}
			}
			return map;
		}
		
		private static int[,] CreateWalls(int[,] map)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			//loop though every grid space
			for (var x = 0; x < roomWidth-1; x++)
			{
				for (var y = 0; y < roomHeight-1; y++)
				{
					//if theres a floor, check the spaces around it
					if ((GridSpace)map[x,y] == GridSpace.Floor)
					{
						//if any surrounding spaces are empty, place a wall
						if ((GridSpace)map[x, y+1] == GridSpace.Empty)
						{
							map[x, y+1] = (int)GridSpace.Wall;
						}
						if ((GridSpace)map[x, y-1] == GridSpace.Empty)
						{
							map[x, y-1] = (int)GridSpace.Wall;
						}
						if ((GridSpace)map[x+1, y] == GridSpace.Empty)
						{
							map[x+1, y] = (int)GridSpace.Wall;
						}
						if ((GridSpace)map[x-1, y] == GridSpace.Empty)
						{
							map[x-1, y] = (int)GridSpace.Wall;
						}
					}
				}
			}
			return map;
		}

		private static int[,] CreateFloors(int[,] map, float seed, float percentToFill)
		{
			var chanceWalkerChangeDir = 0.5f;
			var chanceWalkerSpawn = 0.05f;
			var chanceWalkerDestoy = 0.05f;
			var maxWalkers = 10;
			//Seed our random
			var rand = new System.Random(seed.GetHashCode());
			
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			//find center of grid
			var spawnPos = new Vector2(Mathf.RoundToInt(roomWidth/ 2.0f),
										Mathf.RoundToInt(roomHeight/ 2.0f));
			
			var newWalker = new Walker {Dir = RandomDirection(rand), Pos = spawnPos};
			//add walker to list
			var walkers = new List<Walker> {newWalker};

			var iterations = 0;
			//loop will not run forever
			do
			{
				//create floor at position of every walker
				foreach (var myWalker in walkers)
				{
					map[(int) myWalker.Pos.x, (int) myWalker.Pos.y] = (int)GridSpace.Floor;
				}
				//chance: destroy walker
				var numberChecks = walkers.Count; //might modify count while in this loop
				for (var i = 0; i < numberChecks; i++)
				{
					//only if its not the only one, and at a low chance
					if (NextFloat(rand) < chanceWalkerDestoy && walkers.Count > 1)
					{
						walkers.RemoveAt(i);
						break; //only destroy one per iteration
					}
				}
				//chance: walker pick new direction
				for (var i = 0; i < walkers.Count; i++)
				{
					if (NextFloat(rand) < chanceWalkerChangeDir)
					{
						var thisWalker = walkers[i];
						thisWalker.Dir = RandomDirection(rand);
						walkers[i] = thisWalker;
					}
				}
				//chance: spawn new walker
				numberChecks = walkers.Count; //might modify count while in this loop
				for (var i = 0; i < numberChecks; i++)
				{
					//only if # of walkers < max, and at a low chance
					if (NextFloat(rand) < chanceWalkerSpawn && walkers.Count < maxWalkers)
					{
						//create a walker 
						var tmpWalker = new Walker {Dir = RandomDirection(rand), Pos = walkers[i].Pos};
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
			return map;
		}

		private static int[,] RemoveSingleWalls(int[,] map, float seed, float requiredFloorPercent)
		{
			var roomWidth = map.GetUpperBound(0);
			var roomHeight = map.GetUpperBound(1);
			//loop though every grid space
			for (var x = 0; x < roomWidth-1; x++)
			{
				for (var y = 0; y < roomHeight-1; y++)
				{
					//if theres a wall, check the spaces around it
					if ((GridSpace)map[x,y] == GridSpace.Wall)
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
								if ((GridSpace)map[x + checkX,y+checkY] != GridSpace.Floor)
								{
									allFloors = false;
								}
							}
						}
						if (allFloors)
						{
							map[x,y] = (int)GridSpace.Floor;
						}
					}
				}
			}
			return map;
		}
		
		private static Vector2 RandomDirection(System.Random rand)
		{
			//pick random int between 0 and 3
			var choice = Mathf.FloorToInt(NextFloat(rand) * 3.99f);
			//use that int to chose a direction
			switch (choice)
			{
				case 0:
					return Vector2.down;
				case 1:
					return Vector2.left;
				case 2:
					return Vector2.up;
				default:
					return Vector2.right;
			}
		}

		private static float NextFloat(System.Random random)
		{
			var mantissa = (random.NextDouble() * 2.0) - 1.0;
			// choose -149 instead of -126 to also generate subnormal floats (*)
			var exponent = Math.Pow(2.0, random.Next(-126, 128));
			return (float)(mantissa * exponent);
		}
	
		private static int NumberOfFloors(int [,] map)
		{
			return map.Cast<GridSpace>().Count(space => space == GridSpace.Floor);
		}
	}
}
