using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        public override TileMapSensor CreateTileMapSensor()
        {
            return new TileMapSensor3D(learningEnvironment, behaviorParameters.TeamId, debug);
        }
    }
}
