﻿using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Research.LevelDesign.UnityProcedural.Global_Scripts;
using Unity.Simulation.Games;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Research.LevelDesign.NuclearThrone
{
	public delegate void LevelUpdate();
	public class NuclearThroneLevelGenerator : MonoBehaviour
	{
		[Tooltip("The Tilemap to draw onto")]
		public Tilemap tilemapWalls;
		
		[Tooltip("The Tilemap to draw onto")]
		public Tilemap tilemapGround;
		
		[Tooltip("The Tile to draw (use a RuleTile for best results)")]
		public TileBase tileWall;
		
		[Tooltip("The Tile to draw (use a RuleTile for best results)")]
		public TileBase tileGround;

		[Tooltip("Width of our map")]
		public int width;
		
		[Tooltip("Height of our map")]
		public int height;
		
		[Tooltip("The settings of our map")]
		public MapSettings mapSetting;

		public int players = 2;

		public bool clearInnerWalls = true;

		public GameObject spawnPrefab;

		public GetSpawnProcedural getSpawnPoints;

		public LevelUpdate onLevelUpdate;

		public static int MapIntCounter { get; private set; }

		public List<MapLayer> MapLayerData =>
			new List<MapLayer>
			{
				new MapLayer
				{
					TileMap=tilemapGround,
					Condition=GridSpace.Floor,
					Tile=tileGround
				},
				new MapLayer
				{
					TileMap=tilemapWalls,
					Condition=GridSpace.Wall,
					Tile=tileWall
				}
			};

		public void GenerateMap(int seed)
		{
			InvokeGenerateMap(true, seed);
			onLevelUpdate?.Invoke();
			
			MapIntCounter++;
		}

		private int GetSeed()
		{
			return (int)(gameObject.GetHashCode() * Time.time);
		}

		public void InvokeGenerateMap(bool generateMap)
		{
			var seed = mapSetting.randomSeed ? GetSeed() : mapSetting.seed;
			InvokeGenerateMap(generateMap, seed);
		}

		private void InvokeGenerateMap(bool generateMap, int seed, int distance=2)
		{
			ClearMap();
			seed *= GetHashCode();
			// 
			Random.InitState(seed);
			GameSimManager.Instance.SetCounter("seed", seed);
			
			var validPositions = new List<Vector3Int>();
			var map = NuclearThroneMapFunctions.GenerateArray(width, height);
			while (validPositions.Count < players)
			{
				validPositions.Clear();
				NuclearThroneMapFunctions.ClearArray(map);

				if (generateMap)
				{
					map = NuclearThroneMapGenerator.GenerateMap(map);
				}
				else
				{
					map = NuclearThroneMapGenerator.SquareMap(map);
				}

				if (clearInnerWalls)
				{
					ClearInnerWalls.RemoveInnerWalls(map);
				}

				var z = (int) tilemapGround.transform.position.y;
				for (var y = distance; y < height - distance; y++)
				{
					for (var x = distance; x < width - distance; x++)
					{
						var free = FreeTile(x, y, map, distance);
						var notClose = CheckDistance(x, y, validPositions, 15);
						if (free && notClose)
						{
							validPositions.Add(new Vector3Int(x, y, z));
						}
					}
				}
			}
			
			//Render the result
			NuclearThroneMapFunctions.RenderMapWithOffset(map, MapLayerData);

			var index = 0;
			var spawnPositions = GetMaxDistance(validPositions);
			foreach(var position in spawnPositions)
			{
				var place = tilemapGround.CellToWorld(position);
				var shouldInstantiate = !Application.isPlaying;
				var prefab = shouldInstantiate
					? Instantiate(spawnPrefab)
					: getSpawnPoints.Points[index].gameObject;
				
				getSpawnPoints.AddPoint(index, prefab.GetComponent<MLCheckbox>());
				
				prefab.transform.position = place;
				prefab.transform.parent = getSpawnPoints.transform;

				index++;
			}
		}

		private static IEnumerable<Vector3Int> GetMaxDistance(IReadOnlyCollection<Vector3Int> array)
		{
			var output = new List<Vector3Int>();
			var maxDistance = -1;
			foreach (var item in array)
			{
				foreach (var secondItem in array)
				{
					if (item != secondItem)
					{
						var xDiff = Mathf.Abs(item.x - secondItem.x);
						var yDiff = Mathf.Abs(item.y - secondItem.y);
						var distance = Mathf.Max(xDiff, yDiff);
						if (distance > maxDistance)
						{
							maxDistance = distance;
							output = new List<Vector3Int> {item, secondItem};
						}
					}
				}
			}
			return output;
		}

		private bool CheckDistance(int x, int y, IEnumerable<Vector3Int> availableSpots, int distance)
		{
			foreach (var spot in availableSpots)
			{
				var xDistance = Mathf.Abs(spot.x - x);
				var yDistance = Mathf.Abs(spot.y - y);
				if (xDistance < distance || yDistance < distance)
				{
					return false;
				}
			}

			return true;
		}

		private bool FreeTile(int x, int y, GridSpace[,] map, int distance)
		{
			for (var i = -distance; i <= distance; i++)
			{
				for (var j = -distance; j <= distance; j++)
				{
					if (map[x + i, y + j] != GridSpace.Floor)
					{
						return false;
					}
				}
			}
			return true;
		}

		public void ClearMap()
		{
			tilemapGround.ClearAllTiles();
			tilemapWalls.ClearAllTiles();
			foreach (var point in getSpawnPoints.Points)
			{
				if (!Application.isPlaying)
				{
					DestroyImmediate(point.gameObject);
				}
			}
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(NuclearThroneLevelGenerator))]
	public class NuclearThroneLevelGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			//Reference to our script
			var levelGen = (NuclearThroneLevelGenerator)target;
			
			//Only show the mapsettings UI if we have a reference set up in the editor
			if (levelGen.mapSetting != null)
			{
				Editor mapSettingEditor = CreateEditor(levelGen.mapSetting);
				mapSettingEditor.OnInspectorGUI();

				if (GUILayout.Button("Generate"))
				{
					levelGen.InvokeGenerateMap(true);
				}
				
				if (GUILayout.Button("Generate Square"))
				{
					levelGen.InvokeGenerateMap(false);
				}

				if (GUILayout.Button("Clear"))
				{
					levelGen.ClearMap();
				}
			}
		}
	}
	#endif
}