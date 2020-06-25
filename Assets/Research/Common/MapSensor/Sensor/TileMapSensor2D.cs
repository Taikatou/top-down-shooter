using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        protected override int[] MShape { get; }

        protected override int WriteObservations(ObservationWriter writer)
        {
            var obsSize = TileMapSensorConfigUtils.GetOutputSizeLinear(Config);
            
            // var outputArray = new float[obsSize];
            for (var y = 0; y < Config.ObsSizeY; y++)
            {
                for (var x = 0; x < Config.ObsSizeX; x++)
                {
                    var gridSpace = MObservations[x, y];
                    if (Config.GridSpaceValues.ContainsKey(gridSpace))
                    {
                        var space = (float) gridSpace;
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
            return obsSize;
        }

        public TileMapSensor2D(string name, GetEnvironmentMapPositions environmentInstance, TileMapSensorConfig config, Transform transform) : base(name, environmentInstance, config, transform)
        {
            MShape = config.GetSize(true);
        }
    }
}
