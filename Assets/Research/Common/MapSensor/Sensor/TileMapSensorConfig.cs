using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.Common.MapSensor.Sensor
{
    public struct TileMapSensorConfig
    {
        public int SizeX => Size - 1; 
        public int SizeY => Size - 1;
        public readonly int Size;
        public readonly bool Debug;
        public readonly Dictionary<GridSpace, int> GridSpaceValues;
        public readonly bool TrackPosition;
        public readonly string Name;
        
        public TileMapSensorConfig(int size, bool trackPosition, string name,
            IEnumerable<GridSpace> detectableLayers, bool debug)
        {
            Size = size;
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
