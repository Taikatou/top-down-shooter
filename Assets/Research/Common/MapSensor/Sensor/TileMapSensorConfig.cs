using System.Collections.Generic;
using Research.LevelDesign.Scripts;
using Research.LevelDesign.Scripts.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public struct StartEndPosition
    {
        public Vector2Int StartPos;
        public Vector2Int EndPos;
    }

    public static class TileMapSensorConfigUtils
    {
        public static StartEndPosition GetStartEndPosition(TileMapSensorConfig config)
        {
            var returnValue = new StartEndPosition
            {
                StartPos = new Vector2Int(0, 0), 
                EndPos = new Vector2Int(config.sizeX, config.sizeY)
            };
            
            return ReturnStartEndCompressed(returnValue, config);
        }

        private static StartEndPosition ReturnStartEndCompressed(StartEndPosition position, TileMapSensorConfig config)
        {
            position.StartPos /= config.compressRatio;
            position.EndPos /= config.compressRatio;
            return position;
        }
        
        public static StartEndPosition GetTrackStartEndPosition(TileMapSensorConfig config, Vector3Int cell)
        {
            var halfSize = HalfSize(config);
            
            var returnValue = new StartEndPosition
            {
                StartPos =  new Vector2Int(cell.x, cell.y) - halfSize,
                EndPos = new Vector2Int(cell.x, cell.y) + halfSize
            };

            return ReturnStartEndCompressed(returnValue, config);;
        }

        public static Vector2Int GetMappedPosition(TileMapSensorConfig config, int x, int y, Vector3Int self)
        {
            return new Vector2Int(x - self.x, y - self.y) + HalfSize(config);
        }

        private static Vector2Int HalfSize(TileMapSensorConfig config)
        {
            var halfX = config.sizeX / 2;
            var halfY = config.sizeY / 2;
            return new Vector2Int(halfX, halfY);
        }

        public static int GetOutputSizeLinear(TileMapSensorConfig config)
        {
            return config.ObsSizeX * config.ObsSizeY;
        }
    }
    
    [System.Serializable]
    public struct TileMapSensorConfig
    {
        public int sizeX;
        public int sizeY;
        public int compressRatio;
        public bool debug;
        public bool trackPosition;

        private MapAccessor _mapAccessor;

        public MapAccessor MapAccessor
        {
            get
            {
                if (_mapAccessor == null)
                {
                    _mapAccessor = behaviorParameters.GetComponentInParent<MapAccessor>();
                }

                return _mapAccessor;
            }
        }
        
        public BehaviorParameters behaviorParameters;
        
        public GridSpace[] layerList;

        public int ObsSizeX => sizeX / compressRatio;
        public int ObsSizeY => sizeY / compressRatio;
        
        public int TeamId => behaviorParameters.TeamId;

        private Dictionary<GridSpace, int> _gridSpaceValues;
        
        public Dictionary<GridSpace, int> GridSpaceValues
        {
            get
            {
                if (_gridSpaceValues == null)
                {
                    _gridSpaceValues = new Dictionary<GridSpace, int>();
                    foreach (var layer in layerList)
                    {
                        _gridSpaceValues.Add(layer, _gridSpaceValues.Count);
                    }
                }

                return _gridSpaceValues;
            }
        }

        public int GridSpaceCount => GridSpaceValues.Count;

        public int[] GetSize(bool twoD)
        {
            return new[] { ObsSizeX, ObsSizeY, twoD? 1 : layerList.Length};
        }
    }
}
