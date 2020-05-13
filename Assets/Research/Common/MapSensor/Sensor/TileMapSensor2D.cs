using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        private readonly bool _normalize;

        protected override int WriteObservations(ObservationWriter writer)
        {
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    var gridSpace = MObservations[x, y];
                    if (Config.GridSpaceValues.ContainsKey(gridSpace))
                    {
                        var space = (float) gridSpace;
                        if (_normalize)
                        {
                            writer[x, y, 0] = space / Config.GridSpaceValues.Count;
                        }
                        else
                        {
                            writer[x, y, 0] = space;
                        }
                    }
                }
            }
            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor2D(GameObject learningEnvironment, string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers, bool normalize) : base(learningEnvironment, name, size, trackPosition, debug, detectableLayers)
        {
            _normalize = normalize;
        }
    }
}
