using System;
using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor.SensorData;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
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

        private readonly GetEnvironmentMapPositions _environmentInstance;

        private readonly BaseSensorData _sensorData;

        protected abstract int[] MShape { get; }

        public string GetName() => _name;

        public SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }

        public byte[] GetCompressedObservation()
        {
            return null;
        }

        public int[] GetObservationShape()
        {
            return MShape;
        }

        protected abstract int WriteObservations(ObservationWriter writer);

        public void Reset()
        {
            Array.Clear(MObservations, 0, Config.sizeX * Config.sizeY);
        }

        protected TileMapSensor(string name, GetEnvironmentMapPositions environmentInstance,
            TileMapSensorConfig config, Transform transform)
        {
            _environmentInstance = environmentInstance;
            Config = config;
            _name = name;

            MObservations = new GridSpace[Config.sizeX, Config.sizeY];
            if (config.trackPosition)
            {
                _sensorData = new FollowSensorData(config, transform);
            }
            else
            {
                _sensorData = new FullSensorData(config);
            }
        }

        public int Write(ObservationWriter writer)
        {
            using (TimerStack.Instance.Scoped("TileMapSensor.WriteToTensor"))
            {
                return WriteObservations(writer);
            }
        }

        public void Update()
        {
            _sensorData.UpdateMap(MObservations);
            _sensorData.UpdateMapEntityPositions(MObservations, _environmentInstance.EntityMapPositions);
        }
    }
}
