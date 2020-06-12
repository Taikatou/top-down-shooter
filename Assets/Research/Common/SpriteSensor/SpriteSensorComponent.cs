using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.SpriteSensor
{
    public class SpriteSensorComponent : SensorComponent
    {
        public  SpriteRenderer spriteRenderer;

        private SpriteSensor _spriteSensor;
        public override ISensor CreateSensor()
        {
            _spriteSensor = new SpriteSensor(spriteRenderer);
            return _spriteSensor;
        }

        public override int[] GetObservationShape()
        {
            return new[] { SpriteSensor.Length };
        }
    }
}
