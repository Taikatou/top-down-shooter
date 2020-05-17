using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        protected override ISensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags)
        {
            return new TileMapSensor3D( sensorName, 
                                        tileMapSize, 
                                        trackPosition, 
                                        debug, 
                                        detectTags, 
                                        MapAccessor, 
                                        EnvironmentInstance,
                                        GetTeamId,
                                        buffer);
        }
        
        public override int[] GetObservationShape()
        {
            var shape = TileMapSensor.GetObservationShape();
            return new [] { shape[0], shape[1], shape[2] };
        }
    }
}
