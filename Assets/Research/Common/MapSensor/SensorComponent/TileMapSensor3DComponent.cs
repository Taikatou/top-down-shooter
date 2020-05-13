using Research.Common.MapSensor.Sensor;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        protected override TileMapSensor CreateTileMapSensor()
        {
            return new TileMapSensor3D(learningEnvironment, behaviorParameters.TeamId, debug, detectableLayers);
        }
    }
}
