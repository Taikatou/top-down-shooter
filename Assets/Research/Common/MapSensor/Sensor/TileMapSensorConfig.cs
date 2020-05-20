using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public struct TrackPosition
    {
        public Vector2Int StartPos;
        public Vector2Int EndPos;
    }
    public class TileMapSensorConfig
    {
        private readonly int _size;
        public readonly bool Debug;
        public readonly Dictionary<GridSpace, int> GridSpaceValues;
        public readonly bool TrackPosition;
        public readonly int TeamId;

        public int SizeX => _size - 1;
        public int SizeY => _size - 1;

        public int OutputSizeLinear => (SizeX - _cacheOffset) * (SizeY - _cacheOffset);

        private readonly int _cacheOffset;

        public TrackPosition GetTrackPosition()
        {
            var returnValue = new TrackPosition
            {
                StartPos = new Vector2Int(_cacheOffset, _cacheOffset), 
                EndPos = new Vector2Int(SizeX - _cacheOffset, SizeY - _cacheOffset)
            };

            return returnValue;
        }
        
        public TileMapSensorConfig(int size, bool trackPosition,
            IEnumerable<GridSpace> detectableLayers, bool debug, int teamId, bool buffer)
        {
            _size = size;
            Debug = debug;
            TrackPosition = trackPosition;
            _cacheOffset = buffer ? 1 : 0;
            GridSpaceValues = new Dictionary<GridSpace, int>();
            TeamId = teamId;
            var counter = 0;
            foreach (var layer in detectableLayers)
            {
                GridSpaceValues.Add(layer, counter);
                counter++;
            }
        }
    }
}
