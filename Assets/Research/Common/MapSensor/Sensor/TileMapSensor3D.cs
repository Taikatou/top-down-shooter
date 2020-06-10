using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor3D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            foreach (var pair in Config.GridSpaceValues)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < Config.sizeY; y++)
                {
                    for (var x = 0; x < Config.sizeX; x++)
                    {
                        var isKey = MObservations[x, y] == pair.Key;
                        var present = isKey? 1.0f: 0.0f;
                        writer[x, y, gridInt] = present;
                    }
                }   
            }
            
            if (Config.debug)
            {
                NuclearThroneMapGenerator.OutputDebugMap(MObservations);
            }

            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        private static int[] GetObservationSize3D(TileMapSensorConfig config)
        {
            var detectable = config.GridSpaceValues.Count;
            return new[] { config.sizeX, config.sizeY, detectable };
        }

        public TileMapSensor3D(string name, GetEnvironmentMapPositions environmentInstance, TileMapSensorConfig config) : base(name, environmentInstance, config)
        {
            MShape = GetObservationSize3D(Config);
        }
    }
}
