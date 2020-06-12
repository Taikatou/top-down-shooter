using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            var obsSize = TileMapSensorConfigUtils.GetOutputSizeLinear(Config);
            var trackedPosition = TileMapSensorConfigUtils.GetTrackPosition(Config);
            
            var outputArray = new float[obsSize];
            var index = 0;
            for (var y = trackedPosition.StartPos.y; y < trackedPosition.EndPos.y; y++)
            {
                for (var x = trackedPosition.StartPos.x; x < trackedPosition.EndPos.x; x++)
                {
                    var gridSpace = MObservations[x, y];
                    if (Config.GridSpaceValues.ContainsKey(gridSpace))
                    {
                        var space = (float) gridSpace;
                        outputArray[index] = space;
                    }
                    index++;
                }
            }

            writer.AddRange(outputArray);
            if (Config.debug)
            {
                NuclearThroneMapGenerator.OutputDebugMap(MObservations);
            }
            return obsSize;
        }

        public TileMapSensor2D(string name, GetEnvironmentMapPositions environmentInstance, TileMapSensorConfig config) : base(name, environmentInstance, config)
        {
            MShape = new[] { TileMapSensorConfigUtils.GetOutputSizeLinear(Config) };
        }
    }
}
