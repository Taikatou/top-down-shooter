using System;
using Unity.MLAgents.Sensors;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensorComponent : SensorComponent
    {
        [NonSerialized] private TileMapSensor _tileMapSensor;

        public bool debug;

        public override ISensor CreateSensor()
        {
            _tileMapSensor = new TileMapSensor(gameObject, debug);
            return _tileMapSensor;
        }

        public override int[] GetObservationShape()
        {
            return _tileMapSensor.GetObservationShape();
        }
    }
}
