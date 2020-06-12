using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        private int _outputSizeLinear;
        
        public override ISensor CreateSensor()
        {
            var twoDSensor = new TileMapSensor2D(sensorName,
                                                EnvironmentInstance,
                                                sileMapSensorConfig);
            _outputSizeLinear = TileMapSensorConfigUtils.GetOutputSizeLinear(twoDSensor.Config);

            TileMapSensor = twoDSensor;
            return TileMapSensor;
        }
        
        public override int[] GetObservationShape()
        {
            return new [] { _outputSizeLinear };
        }
    }
}
