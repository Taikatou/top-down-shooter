﻿using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        public override ISensor CreateSensor()
        {
            TileMapSensor = new TileMapSensor3D(sensorName,
                                                EnvironmentInstance,
                                                tileMapSensorConfig,
                                                transform);
            return TileMapSensor;
        }
        
        public override int[] GetObservationShape()
        {
            return tileMapSensorConfig.GetSize(false);
        }
    }
}
