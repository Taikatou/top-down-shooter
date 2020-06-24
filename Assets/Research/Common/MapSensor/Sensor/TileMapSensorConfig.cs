using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
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
            var halfX = config.sizeX / 2;
            var halfY = config.sizeY / 2;
            var returnValue = new StartEndPosition
            {
                StartPos =  new Vector2Int(cell.x - halfX, cell.y-halfY),
                EndPos = new Vector2Int(cell.x + halfX, cell.y+halfY)
            };

            return ReturnStartEndCompressed(returnValue, config);;
        }

        public static Vector2Int GetMappedPosition(TileMapSensorConfig config, int x, int y, Vector3Int self)
        {
            var halfX = config.sizeX / 2;
            var halfY = config.sizeY / 2;
            
            return new Vector2Int(x - self.x + halfX, y - self.y + halfY);
        }

        public static int GetOutputSizeLinear(TileMapSensorConfig config)
        {
            var size = config.ObsSizeX * config.ObsSizeY;

            return size;
        }
    }
    
    [System.Serializable]
    public struct TileMapSensorConfig
    {
        public bool debug;
        public GridSpace[] layerList;
        public BehaviorParameters behaviorParameters;
        
        public MapAccessor mapAccessor;
        public int sizeX;
        public int sizeY;

        public bool trackPosition;

        public int compressRatio;

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
    }
}
