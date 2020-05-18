using System.Collections.Generic;
using System.Linq;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        protected override int[] MShape => new[] { Config.OutputSizeLinear };

        protected override int WriteObservations(ObservationWriter writer)
        {
            var obsSize = Config.OutputSizeLinear;
            var outputArray = new float[obsSize];
            var trackedPosition = Config.GetTrackPosition();
            
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
            if (Config.Debug)
            {
                // OutputDebugMap(MObservations);
                // var debugTxt = string.Join(", ", outputArray);
            }
            return obsSize;
        }

        public TileMapSensor2D(string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers, MapAccessor mapAccessor, EnvironmentInstance environmentInstance, int teamId, bool buffer) : base(name, size, trackPosition, debug, detectableLayers, mapAccessor, environmentInstance, teamId, buffer)
        {
        }
    }
}
