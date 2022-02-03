using System;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.LevelDesign.Scripts.MLAgents
{
    public class TileMapGridSensor : IGridPerception
    {
        private readonly GameObject m_CenterObject; 
        private readonly Vector2Int m_GridSize;
        private readonly Vector2 m_CellCenterOffset;
        private readonly TileMapSensorConfig Config;
        public bool RotateWithAgent { get; set; }
        public LayerMask ColliderMask { get; set; }

        private readonly Vector3[] m_CellLocalPositions;

        public event Action<GridSpace, int> GridOverlapDetectedAll;

        private int mNumCells;
        
        private readonly GetEnvironmentMapPositions _environmentInstance;

        public TileMapGridSensor(Vector2Int gridSize, GameObject centerObject, TileMapSensorConfig config, GetEnvironmentMapPositions environmentInstance)
        { 
            m_GridSize = new Vector2Int(gridSize.x, gridSize.y);
            m_CenterObject = centerObject; 
            Config = config;
            _environmentInstance = environmentInstance;

            m_CellCenterOffset = new Vector2((gridSize.x - 1f) / 2, (gridSize.y - 1f) / 2);
            
            mNumCells = (gridSize.x * gridSize.y);
            m_CellLocalPositions = new Vector3[mNumCells];

            for (var i = 0; i < mNumCells; i++)
            {
                m_CellLocalPositions[i] = GetCellLocalPosition(i);
            }
        }
        
        public Vector3 GetCellLocalPosition(int cellIndex)
        {
            var x = (cellIndex / m_GridSize.x - m_CellCenterOffset.x);
            var z = (cellIndex % m_GridSize.y - m_CellCenterOffset.y);
            var cellPosition = new Vector3(x, z, 0);
            return cellPosition;
        }

        public Vector3 GetCellGlobalPosition(int cellIndex)
        {
            return m_CellLocalPositions[cellIndex%mNumCells] + m_CenterObject.transform.position;
        }

        public Quaternion GetGridRotation()
        {
            return Quaternion.identity;
        }
        
        public void Perceive()
        {
            // NuclearThroneMapGenerator.OutputDebugMap(observations);
            var map = Config.MapAccessor.GetMap();
            UpdateMapEntityPositions(map, _environmentInstance.EntityMapPositions);
            
            var agentCell = Config.MapAccessor.GetPosition(m_CenterObject.transform.position);

            var trackedPosition = TileMapSensorConfigUtils.GetTrackStartEndPosition(Config, agentCell);

            int Y(int i) => trackedPosition.StartPos.y + i;
            int X(int j) => trackedPosition.StartPos.x + j;
            for (var i = 0; i < m_GridSize.y; i++)
            {
                for (var j = 0; j < m_GridSize.x; j++)
                {
                    var y = Y(i);
                    var x = X(j);
                    var xyValid = ValidSpace(map, x, y); 
                    if (xyValid && XyValid(x, y, trackedPosition))
                    {
                        var cellIndex = i * m_GridSize.y + j;
                        Debug.Log(map[x, y]);
                        GridOverlapDetectedAll?.Invoke(map[x, y], cellIndex);
                    }
                }
            }
        }

        private void UpdateMapEntityPositions(GridSpace[,] observations, BaseMapPosition[] entityMapPositions)
        {
            foreach (var entityList in entityMapPositions)
            {
                Debug.Log(entityList == null);
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    var cell = Config.MapAccessor.GetPosition(entity.Position);
                    var trackPos = TileMapSensorConfigUtils.GetStartEndPosition(Config);

                    var xValid = cell.x >= trackPos.StartPos.x && cell.x < trackPos.EndPos.x;
                    var yValid = cell.y >= trackPos.StartPos.y && cell.y < trackPos.EndPos.y;

                    if (xValid && yValid)
                    {
                        var gridType = entity.GridSpace;
                        var contains = Config.GridSpaceValues.ContainsKey(gridType);
                        if (contains)
                        {
                            observations[cell.x, cell.y] = gridType;
                        }
                    }
                }
            }
        }
        
        private bool XyValid(int x, int y, StartEndPosition startEndPosition)
        {
            return x >= startEndPosition.StartPos.x && x <= startEndPosition.EndPos.x &&
                   y >= startEndPosition.StartPos.y && y <= startEndPosition.EndPos.y;
        }

        private static bool ValidSpace(GridSpace[,] map, int x, int y, int beginX=0, int beginY=0)
        {
            return x >= beginX && x < map.GetUpperBound(0) &&
                   y >= beginY && y < map.GetUpperBound(1);
        }

        private static bool ValidSpace(GridSpace[,] map, Vector2Int position)
        {
            return ValidSpace(map, position.x, position.y);
        }

        public void UpdateGizmo()
        {
            
        }

        public void RegisterSensor(GridSensorBase sensor)
        {
            var gridMapSensor = (GridMapSensor) sensor;
            GridOverlapDetectedAll += gridMapSensor.ProcessDetectedGrid;
        }

        public void RegisterDebugSensor(GridSensorBase debugSensor)
        {
            
        }
    }
}
