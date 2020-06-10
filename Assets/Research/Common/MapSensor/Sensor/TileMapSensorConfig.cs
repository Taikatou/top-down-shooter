using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public struct TrackPosition
    {
        public Vector2Int StartPos;
        public Vector2Int EndPos;
    }

    public static class TileMapSensorConfigUtils
    {
        public static TrackPosition GetTrackPosition(TileMapSensorConfig config)
        {
            var returnValue = new TrackPosition
            {
                StartPos = new Vector2Int(0, 0), 
                EndPos = new Vector2Int(config.sizeX, config.sizeY)
            };

            return returnValue;
        }

        public static int GetOutputSizeLinear(TileMapSensorConfig config)
        {
            return config.sizeX * config.sizeY;
        }
    }
    
    [System.Serializable]
    public struct TileMapSensorConfig
    {
        public bool debug;
        public GridSpace[] layerList;
        public GridSpace[] selfList;
        public GridSpace[] otherList;
        public BehaviorParameters behaviorParameters;
        
        public MapAccessor mapAccessor;
        public int sizeX;
        public int sizeY;
        
        public int TeamId => behaviorParameters.TeamId;

        private Dictionary<GridSpace, int> _gridSpaceValues;
        
        public Dictionary<GridSpace, int> GridSpaceValues
        {
            get
            {
                if (_gridSpaceValues == null)
                {
                    _gridSpaceValues = new Dictionary<GridSpace, int>();
                    AddToList(layerList);
                    AddToList(TeamId == 0 ? selfList : otherList);
                    AddToList(TeamId == 1 ? selfList : otherList);
                }

                return _gridSpaceValues;
            }
        }

        public void AddToList(GridSpace[] list)
        {
            foreach (var layer in list)
            {
                _gridSpaceValues.Add(layer, _gridSpaceValues.Count);
            }
        }
    }
}
