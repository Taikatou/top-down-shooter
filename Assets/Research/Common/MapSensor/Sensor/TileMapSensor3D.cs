using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor
{
    public class TileMapSensor3D : TileMapSensor
    {
        public override int WriteObservations(ObservationWriter writer)
        {
            foreach (var pair in GridSpaceValues)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        var isKey = MObservations[x, y] == pair.Key;
                        var present = isKey? 1.0f: 0.0f;
                        writer[x, y, gridInt] = present;
                    }
                }   
            }
            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor3D(GameObject learningEnvironment, int teamId, bool debug) : base(learningEnvironment, teamId, debug)
        {
        }
    }
}
