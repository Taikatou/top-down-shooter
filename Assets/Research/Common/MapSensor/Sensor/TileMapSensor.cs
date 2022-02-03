using System;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common.MapSensor.Sensor.SensorData;
using Research.LevelDesign.Scripts.MLAgents;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public abstract class TileMapSensor : ISensor
    {
        protected readonly TileMapSensorConfig Config;
        
        protected readonly GridSpace[,] MObservations;

        private readonly string _name;

        private readonly BaseSensorData _sensorData;

        protected ObservationSpec MObservationSpec;
        
        protected abstract int[] MShape { get; }

        public string GetName() => _name;
        
        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public byte[] GetCompressedObservation()
        {
            return null;
        }

        protected abstract int WriteObservations(ObservationWriter writer);

        public void Reset()
        {
            Array.Clear(MObservations, 0, TileMapSensorConfigUtils.GetOutputSizeLinear(Config));
        }

        protected TileMapSensor(string name,
            ref TileMapSensorConfig config, Transform transform)
        {
            Config = config;
            _name = name;

            MObservations = new GridSpace[Config.ObsSizeX, Config.ObsSizeY];

            if (config.trackPosition)
            {
                _sensorData = new FollowSensorData(ref config, transform);
            }
            else
            {
                _sensorData = new FullSensorData(ref config);
            }
        }

        public ObservationSpec GetObservationSpec()
        {
            return MObservationSpec;
        }

        public int Write(ObservationWriter writer)
        {
            return WriteObservations(writer);
        }

        private EnvironmentInstance _instance;
        public void Update()
        {
            using (TimerStack.Instance.Scoped("Update tilemap sensor"))
            {
                if (!_instance)
                {
                    _instance = Config.behaviorParameters.GetComponentInParent<EnvironmentInstance>();
                }

                if (_instance)
                {
                    _sensorData.UpdateMap(MObservations);
                    _sensorData.UpdateMapEntityPositions(MObservations, _instance.EntityMapPositions);
                }
            }
        }
    }
}
