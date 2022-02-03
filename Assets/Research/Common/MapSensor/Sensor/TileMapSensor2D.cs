using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            // var outputArray = new float[obsSize];
            for (var y = 0; y < Config.ObsSizeY; y++)
            {
                for (var x = 0; x < Config.ObsSizeX; x++)
                {
                    var gridSpace = MObservations[x, y];
                    if (Config.GridSpaceValues.ContainsKey(gridSpace))
                    {
                        var space = (float)Config.GridSpaceValues[gridSpace] / (float)Config.GridSpaceCount;
                        // outputArray[index] = space;
                        writer[x, y, 0] = space;
                    }
                }
            }
            
            // writer.AddRange(outputArray);
            if (Config.debug)
            {
                NuclearThroneMapGenerator.OutputDebugMap(MObservations);
            }
            return MShape[0] * MShape[1];
        }

        public TileMapSensor2D(string name, ref TileMapSensorConfig config, Transform transform) : base(name, ref config, transform)
        {
            var shape = config.GetSize(true);
            MObservationSpec = ObservationSpec.Visual(shape[0], shape[1], 1);
            MShape = shape;
        }
    }
}
