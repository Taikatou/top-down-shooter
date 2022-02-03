using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.SpriteSensor
{
    public class SpriteSensorComponent : SensorComponent
    {
        public  SpriteRenderer spriteRenderer;

        private SpriteSensor _spriteSensor;

        public override ISensor[] CreateSensors()
        {
            return new[]
            {
                new SpriteSensor(spriteRenderer)
            };
        }
    }
}
