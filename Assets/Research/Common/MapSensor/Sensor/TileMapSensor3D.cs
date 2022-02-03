using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor3D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            using (TimerStack.Instance.Scoped("WriteObservations"))
            {
                foreach (var pair in Config.GridSpaceValues)
                {
                    var gridInt = pair.Value;
                    for (var y = 0; y < Config.sizeY; y++)
                    {
                        for (var x = 0; x < Config.sizeX; x++)
                        {
                            var isKey = MObservations[x, y] == pair.Key;
                            var present = isKey ? 1.0f : 0.0f;
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
        }

        public TileMapSensor3D(string name, ref TileMapSensorConfig config, Transform transform) : base(name, ref config, transform)
        {
            var size = config.GetSize(false);
            MObservationSpec = ObservationSpec.Visual(size[0], size[1], size[2]);

            MShape = size;
        }
    }
}
