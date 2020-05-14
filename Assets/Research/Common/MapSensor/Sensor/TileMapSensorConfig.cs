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
        public readonly string Name;

        public int SizeX => _size - 1; 
        public int SizeY => _size - 1;

        private int HalfX => _size / 2;
        private int HalfY => _size / 2;

        public TrackPosition GetTrackPositionPosition(Vector3Int position)
        {
            var startX = position.x - HalfX;
            var startY = position.y - HalfY;
            var endX = position.x + HalfX;
            var endY = position.y + HalfY;
            return new TrackPosition()
            {
                StartPos = new Vector2Int(startX, startY),
                EndPos = new Vector2Int(endX, endY)
            };
        }
        
        public TileMapSensorConfig(int size, bool trackPosition, string name,
            IEnumerable<GridSpace> detectableLayers, bool debug)
        {
            _size = size;
            Debug = debug;
            Name = name;
            TrackPosition = trackPosition;
            GridSpaceValues = new Dictionary<GridSpace, int>();

            var counter = 0;
            foreach (var layer in detectableLayers)
            {
                GridSpaceValues.Add(layer, counter);
                counter++;
            }
        }
    }
}
