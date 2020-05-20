using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor3D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            var debugGridSpace = new GridSpace[Config.SizeX, Config.SizeY];
            foreach (var pair in Config.GridSpaceValues)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < Config.SizeY; y++)
                {
                    for (var x = 0; x < Config.SizeX; x++)
                    {
                        var isKey = MObservations[x, y] == pair.Key;
                        var present = isKey? 1.0f: 0.0f;
                        writer[x, y, gridInt] = present;

                        if (isKey)
                        {
                            debugGridSpace[x, y] = MObservations[x, y];   
                        }
                    }
                }   
            }
            
            if (Config.Debug)
            {
                OutputDebugMap(debugGridSpace);
            }

            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor3D(string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers, MapAccessor mapAccessor, GetEnvironmentMapPositions environmentInstance, int teamId, bool buffer) : base(name, size, trackPosition, debug, detectableLayers, mapAccessor, environmentInstance, teamId, buffer)
        {
            MShape = GetObservationSize3D(Config);
        }

        private static int[] GetObservationSize3D(TileMapSensorConfig config)
        {
            var detectable = config.GridSpaceValues.Count;
            return new[] { config.SizeX, config.SizeY, detectable };
        }

        public static int[] GetObservationSize3D(int size, IEnumerable<GridSpace> detectableLayers)
        {
            var config = new TileMapSensorConfig(size, false, detectableLayers, false, 1, false);
            return GetObservationSize3D(config);
        }
    }
}
