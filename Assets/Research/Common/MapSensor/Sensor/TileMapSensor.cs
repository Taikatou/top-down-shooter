using System;
using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor.SensorData;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public abstract class TileMapSensor : ISensor
    {
        protected readonly TileMapSensorConfig Config;
        
        protected readonly GridSpace[,] MObservations;

        private readonly GridSpace[,] _observationRaw;
        
        private readonly string _name;

        private readonly GetEnvironmentMapPositions _environmentInstance;

        private readonly BaseSensorData _sensorData;

        private bool _compress;

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
            Array.Clear(MObservations, 0, TileMapSensorConfigUtils.GetOutputSizeLinear(Config));
        }

        protected TileMapSensor(string name, GetEnvironmentMapPositions environmentInstance,
            TileMapSensorConfig config, Transform transform)
        {
            _environmentInstance = environmentInstance;
            Config = config;
            _name = name;

            var observationX = Config.sizeX / Config.compressRatio;
            var observationY = Config.sizeY / Config.compressRatio;

            MObservations = new GridSpace[observationX, observationY];
            _compress = Config.compressRatio > 1;

            if (_compress)
            {
                _observationRaw = new GridSpace[Config.sizeX, Config.sizeY];   
            }
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

        private static readonly Dictionary<GridSpace, int> GridSpacePriority = new Dictionary<GridSpace, int>
        {
            { GridSpace.Empty, 0 },
            { GridSpace.Floor, 1 },
            { GridSpace.Wall, 2 },
            { GridSpace.Team1, 3 },
            { GridSpace.Team2, 4 },
            { GridSpace.Projectile1, 5 },
            { GridSpace.Projectile2, 6 },
            { GridSpace.Health, 7 }
        };

        private void CompressObservations()
        {
            var yO = 0;
            Debug.Log(_observationRaw.GetUpperBound(1) + "\t" + _observationRaw.GetUpperBound(0));
            for (var y = 0; y <= _observationRaw.GetUpperBound(1); y += Config.compressRatio, yO++)
            {
                var xO = 0;
                for (var x = 0; x <= _observationRaw.GetUpperBound(0); x += Config.compressRatio, xO++)
                {
                    var pooledData = GridSpace.Empty;
                    for (var j = 0; j < Config.compressRatio; j++)
                    {
                        for (var i = 0; i < Config.compressRatio; i++)
                        {
                            var space = _observationRaw[x + i, y + j];
                            if (GridSpacePriority.ContainsKey(space))
                            {
                                if (GridSpacePriority[space] > GridSpacePriority[pooledData])
                                {
                                    pooledData = space;
                                }
                            }
                            else
                            {
                                Debug.Log(space);
                            }
                        }
                    }
                    

                    if (FollowSensorData.ValidSpace(MObservations, xO, yO))
                    {
                        MObservations[xO, yO] = pooledData;
                    }
                }
            }
        }
        
        public void Update()
        {
            if (_compress)
            {
                _sensorData.UpdateMap(_observationRaw);
                _sensorData.UpdateMapEntityPositions(_observationRaw, _environmentInstance.EntityMapPositions);
                CompressObservations();   
            }
            else
            {
                _sensorData.UpdateMap(MObservations);
                _sensorData.UpdateMapEntityPositions(MObservations, _environmentInstance.EntityMapPositions);
            }
        }
    }
}
