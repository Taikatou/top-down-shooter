using Research.Common.MapSensor.Sensor;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        public bool twoDSensor;
        public int stackObservation = 3;
        public override ISensor[] CreateSensors()
        {
            var sensor = twoDSensor
                ? (ISensor)new TileMapSensor2D(sensorName, ref tileMapSensorConfig, transform)
                : (ISensor)new TileMapSensor3D(sensorName, ref tileMapSensorConfig, transform);
            
            if (stackObservation > 1)
            {
                sensor = new StackingSensor(sensor, stackObservation);
            } 
            return new[] { sensor };
        }
    }
}
