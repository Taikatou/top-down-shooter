using System;
using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.Sensor
{
    public abstract class TileMapSensor : ISensor
    {
        private readonly string _name;

        protected readonly GridSpace[,] MObservations;

        public readonly TileMapSensorConfig Config;

        private readonly GetEnvironmentMapPositions _environmentInstance;

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
            TileMapSensorConfig config)
        {
            _environmentInstance = environmentInstance;
            Config = config;
            _name = name;

            MObservations = new GridSpace[Config.sizeX, Config.sizeY];
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
            UpdateMap();
            UpdateMapEntityPositions();
        }

        private void UpdateMap()
        {
            var map = Config.mapAccessor.GetMap();
            var startEnd = TileMapSensorConfigUtils.GetTrackPosition(Config);
            for (var y = startEnd.StartPos.y; y < startEnd.EndPos.y; y++)
            {
                for (var x = startEnd.StartPos.x; x < startEnd.EndPos.x; x++)
                {
                    var value = map[x, y];
                    MObservations[x, y] = value;
                }
            }
        }

        private void UpdateMapEntityPositions()
        {
            foreach (var entityList in _environmentInstance.EntityMapPositions)
            {
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    var cell = Config.mapAccessor.GetPosition(entity.Position);
                    var trackPos = TileMapSensorConfigUtils.GetTrackPosition(Config);

                    var xValid = cell.x >= trackPos.StartPos.x && cell.x < trackPos.EndPos.x;
                    var yValid = cell.y >= trackPos.StartPos.y && cell.y < trackPos.EndPos.y;

                    if (xValid && yValid)
                    {
                        var gridType = entity.GridSpace;
                        var contains = Config.GridSpaceValues.ContainsKey(gridType);
                        if (contains)
                        {
                            MObservations[cell.x, cell.y] = gridType;
                        }
                    }
                }
            }
        }
    }
}
