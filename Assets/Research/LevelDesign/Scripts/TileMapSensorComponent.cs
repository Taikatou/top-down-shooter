using System;
using MLAgents.Sensors;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensorComponent : SensorComponent
    {
        [NonSerialized] private TileMapSensor _tileMapSensor;

        public override ISensor CreateSensor()
        {
            _tileMapSensor = new TileMapSensor(gameObject);
            return _tileMapSensor;
        }

        public override int[] GetObservationShape()
        {
            return _tileMapSensor.GetObservationShape();
        }
    }
}
