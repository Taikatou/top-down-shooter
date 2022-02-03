using Research.Common.MapSensor.Sensor;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        public override ISensor[] CreateSensors()
        {
            return new ISensor[]
            {
                new TileMapSensor2D(sensorName,
                    ref tileMapSensorConfig,
                    transform)
            };
        }
    }
}
