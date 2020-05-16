using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
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

            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor2D(string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers, MapAccessor mapAccessor, EnvironmentInstance environmentInstance, int teamId) : base(name, size, trackPosition, debug, detectableLayers, mapAccessor, environmentInstance, teamId)
        {
        }
    }
}
