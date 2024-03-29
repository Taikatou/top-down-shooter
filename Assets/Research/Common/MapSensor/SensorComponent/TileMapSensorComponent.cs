﻿using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public abstract class TileMapSensorComponent : Unity.MLAgents.Sensors.SensorComponent
    {
        public string sensorName;
        
        public TileMapSensorConfig tileMapSensorConfig;
    }
}
