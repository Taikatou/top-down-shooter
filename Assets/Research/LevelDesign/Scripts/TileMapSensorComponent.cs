using System;
using MLAgents.Sensors;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensorComponent : SensorComponent
    {
        [NonSerialized] private TileMapSensor _tileMapSensor;

        public override ISensor CreateSensor()
        {
            var size = 50;
            _tileMapSensor = new TileMapSensor(gameObject, size, size);
            return _tileMapSensor;
        }

        public override int[] GetObservationShape()
        {
            return _tileMapSensor.GetObservationShape();
        }
    }
}
