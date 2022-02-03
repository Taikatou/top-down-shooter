using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using Research.CharacterDesign.Scripts.SpawnPoints;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using Research.LevelDesign.UnityProcedural.Global_Scripts;
using Unity.Simulation.Games;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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

		public bool clearInnerWalls = true;

		public GetSpawnProcedural getSpawnPoints;

		public GetPickupProcedural pickupProcedural;

		public LevelUpdate OnLevelUpdate;

		[HideInInspector]
		public int mapSeed;

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

		public void GenerateMap(int seed, bool generateMap=true)
		{
			InvokeGenerateMap(generateMap, seed);
			mapSeed = seed;
			
			OnLevelUpdate?.Invoke();
		}

		public GridSpace[,] InvokeGenerateMap(bool generateMap, int seed)
		{
			ClearMap();
			Random.InitState(seed);
			if (MlLevelManager.UnitySimulation)
			{
				GameSimManager.Instance.SetCounter("seed", seed);
			}

			var z = (int) tilemapGround.transform.position.y;

			var map = NuclearThroneMapFunctions.GenerateArray(width, height);
			var spawnPositions = new List<Vector3Int>();
			while (spawnPositions.Count < getSpawnPoints.Players)
			{
				spawnPositions.Clear();
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

				spawnPositions = getSpawnPoints.GetLocations(map, z);
			}

			// Render the result
			NuclearThroneMapFunctions.RenderMapWithOffset(map, MapLayerData);
			
			Debug.Log("Spawn Positions");
			var spawnPoints = SpawnPoints(spawnPositions, getSpawnPoints);
			AddToMap(map, spawnPoints);

			var healthPositions = pickupProcedural.GetHealthPositions(map, spawnPositions, z);
			var pickups = SpawnPoints(healthPositions, pickupProcedural);
			AddToMap(map, pickups);

			FindObjectOfType<DataLogger>()?.OutputMap(map, seed);

			return map;
		}

		private void AddToMap<T>(GridSpace[,] map, T[] points) where T : MonoBehaviour, IEntityClass
		{
			foreach (var point in points)
			{
				var cell = GetPosition(point.transform.position);
				map[cell.x, cell.y] = point.GetGridSpace();
			}
		}
		
		public Vector3Int GetPosition(Vector3 position)
		{
			return tilemapGround.WorldToCell(position);
		}

		private T[] SpawnPoints<T>(List<Vector3Int> spawnPositions, GetEntityProcedural<T> proceduralPoint) where T : MonoBehaviour, IEntityClass
		{
			var spawnedItems = new T[spawnPositions.Count];
			var index = 0;
			foreach(var position in spawnPositions)
			{
				var prefab = SpawnGameObject(proceduralPoint, position, index);
				var inter = prefab.GetComponent<T>();
				spawnedItems[index] = inter;
				
				proceduralPoint.AddPoint(index, inter);
				inter.SetId(index);
				
				prefab.transform.parent = proceduralPoint.transform;

				index++;
			}
			return spawnedItems;
		}

		private GameObject SpawnGameObject<T>(GetEntityProcedural<T> proceduralPoint, Vector3Int position, int index) where T : MonoBehaviour, IEntityClass
		{
			var place = tilemapGround.CellToWorld(position);
			place.x += 0.5f;
			place.y += 0.5f;

			var prefab = Instantiate(proceduralPoint.entityPrefab);
				
			prefab.transform.position = place;

			return prefab;
		}

		public void ClearMap()
		{
			tilemapGround.ClearAllTiles();
			tilemapWalls.ClearAllTiles();
			getSpawnPoints.DestroyEntity();
			pickupProcedural.DestroyEntity();
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(NuclearThroneLevelGenerator))]
	public class NuclearThroneLevelGeneratorEditor : Editor
	{
		public int seed;
		public bool randomSeed;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			//Reference to our script
			var levelGen = (NuclearThroneLevelGenerator)target;
			
			//mapLayer.randomSeed = EditorGUILayout.Toggle("Random Seed", mapLayer.randomSeed);

			randomSeed = EditorGUILayout.Toggle("Random Seed", randomSeed);
			//Only appear if we have the random seed set to false
			if (!randomSeed)
			{
				seed = EditorGUILayout.IntField("Seed", seed);
			}
			
			//Only show the mapsettings UI if we have a reference set up in the editor
			if (levelGen.mapSetting != null)
			{
				Editor mapSettingEditor = CreateEditor(levelGen.mapSetting);
				mapSettingEditor.OnInspectorGUI();
				
				var newSeed = randomSeed ? (int)System.DateTime.Now.Ticks : seed;

				if (GUILayout.Button("Generate"))
				{
					levelGen.InvokeGenerateMap(true, newSeed);
				}
				
				if (GUILayout.Button("Generate Square"))
				{
					levelGen.InvokeGenerateMap(false, newSeed);
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