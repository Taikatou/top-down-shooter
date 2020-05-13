using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor3D : TileMapSensor
    {
        protected override int WriteObservations(ObservationWriter writer)
        {
            var debugGridSpace = new GridSpace[SizeX, SizeY];
            foreach (var pair in Config.GridSpaceValues)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
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
            else
            {
                Debug.Log("No debug trace");
            }
            
            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor3D(GameObject learningEnvironment, string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers) : base(learningEnvironment, name, size, trackPosition, debug, detectableLayers)
        {
        }
    }
}
