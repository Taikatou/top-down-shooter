﻿using Research.Common.MapSensor.Sensor;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        public bool normalize;
        public override TileMapSensor CreateTileMapSensor()
        {
            return new TileMapSensor2D(learningEnvironment, behaviorParameters.TeamId, debug, normalize);
        }
    }
}