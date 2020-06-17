using Research.Common.MapSensor.Sensor;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        public override ISensor CreateSensor()
        {
            var twoDSensor = new TileMapSensor2D(sensorName,
                                                EnvironmentInstance,
                                                tileMapSensorConfig,
                                                transform);

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
