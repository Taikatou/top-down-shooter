﻿using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.UnityProcedural.Global_Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Research.LevelDesign.NuclearThrone
{
	#if UNITY_EDITOR
#endif

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

		public GameObject spawnPrefab;

		public List<GameObject> spawnGameObjects;
		
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.N))
			{
				ClearMap();
				GenerateMap();
			}
		}

		[ExecuteInEditMode]
		public void GenerateMap()
		{
			ClearMap();
			
			var seed = mapSetting.randomSeed ? Time.time : mapSetting.seed;
			Random.InitState(seed.GetHashCode());

			var validPositions = new List<Vector3Int>();
			var map = NuclearThroneMapFunctions.GenerateArray(width, height);
			while (validPositions.Count < players)
			{
				validPositions.Clear();
				NuclearThroneMapFunctions.ClearArray(map, true);

				map = NuclearThroneMapGenerator.GenerateMap(map);

				var distance = 2;
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
			NuclearThroneMapFunctions.RenderMapWithOffset(map, tilemapGround, tilemapWalls, tileWall, tileGround);

			var spawnPositions = GetMaxDistance(validPositions);
			foreach(var position in spawnPositions)
			{
				var place = tilemapGround.CellToWorld(position);
				var prefab = Instantiate(spawnPrefab, place, Quaternion.identity);
				prefab.transform.parent = tilemapGround.transform;
				spawnGameObjects.Add(prefab);
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
			foreach (var point in spawnGameObjects)
			{
				DestroyImmediate(point);
			}
			spawnGameObjects.Clear();
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
					levelGen.GenerateMap();
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