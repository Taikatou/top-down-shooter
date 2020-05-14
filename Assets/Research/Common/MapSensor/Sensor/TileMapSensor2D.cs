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
            var debugGridSpace = new GridSpace[Config.SizeX, Config.SizeY];
            for (var y = 0; y < Config.SizeY; y++)
            {
                for (var x = 0; x < Config.SizeX; x++)
                {
                    var gridSpace = MObservations[x, y];
                    if (Config.GridSpaceValues.ContainsKey(gridSpace))
                    {
                        var space = (float) gridSpace;
                        
                        if (_normalize)
                        {
                            space /= Config.GridSpaceValues.Count;
                        }
                        writer[x, y, 0] = space;
                        if (Config.Debug)
                        {
                            debugGridSpace[x, y] = gridSpace;
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

        public TileMapSensor2D(GameObject learningEnvironment, string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers, bool normalize) : base(learningEnvironment, name, size, trackPosition, debug, detectableLayers)
        {
            _normalize = normalize;
        }
    }
}
