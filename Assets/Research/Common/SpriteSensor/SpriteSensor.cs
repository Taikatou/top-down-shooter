using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.SpriteSensor
{
    public class SpriteSensor : ISensor
    {
        private const string Name = "SpriteSensor";

        private readonly float[] _mObservations;

        private readonly SpriteRenderer _spriteRenderer;

        public SpriteSensor(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
            _mObservations = new float [SpriteId.Instance.Length];
        }
        // Update is called once per frame
        public int[] GetObservationShape()
        {
            return new[] {_mObservations.Length};
        }

        public int Write(ObservationWriter writer)
        {
            writer.AddRange(_mObservations);
            return _mObservations.Length;
        }

        public byte[] GetCompressedObservation()
        {
            return null;
        }

        public void Update()
        {
            var id = SpriteId.Instance.GetId(_spriteRenderer);
            
            for(var index = 0; index < id.Length; index++)
            {
                _mObservations[index] = id[index];
            }  
        }

        public void Reset()
        {
            for (var index = 0; index < _mObservations.Length; index++)
            {
                _mObservations[index] = 0;
            }
        }

        public SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }

        public string GetName()
        {
            return Name;
        }
    }
}
