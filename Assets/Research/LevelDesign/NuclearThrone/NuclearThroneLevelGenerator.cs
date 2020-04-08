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

		public MLCheckbox spawnPrefabs;
		
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
			float seed;
			if (mapSetting.randomSeed)
			{
				seed = Time.time;
			}
			else
			{
				seed = mapSetting.seed;
			}
			
			var map = NuclearThroneMapFunctions.GenerateArray(width, height, true);
			map = NuclearThroneMapGenerator.GenerateMap(map, seed);

			//Render the result
			NuclearThroneMapFunctions.RenderMapWithOffset(map, tilemapGround, tilemapWalls, tileWall, tileGround);
		}

		public void ClearMap()
		{
			tilemapGround.ClearAllTiles();
			tilemapWalls.ClearAllTiles();
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