using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        
        public override ISensor CreateSensor()
        {
            var twoDSensor = new TileMapSensor2D(sensorName,
                                                EnvironmentInstance,
                                                tileMapSensorConfig);

            TileMapSensor = twoDSensor;
            return TileMapSensor;
        }
        
        public override int[] GetObservationShape()
        {
            var outputSizeLinear = TileMapSensorConfigUtils.GetOutputSizeLinear(tileMapSensorConfig);
            return new [] { outputSizeLinear };
        }
    }
}
