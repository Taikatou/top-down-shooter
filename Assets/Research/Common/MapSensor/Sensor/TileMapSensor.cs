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

        protected readonly GetEnvironmentMapPositions EnvironmentInstance;

        protected readonly MapAccessor MapAccessor;

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
            Array.Clear(MObservations, 0, Config.SizeX * Config.SizeY);
        }

        protected TileMapSensor(string name, int size, bool trackPosition, bool debug,
            IEnumerable<GridSpace> detectableLayers,
            MapAccessor mapAccessor, GetEnvironmentMapPositions environmentInstance, int teamId, bool buffer)
        {
            MapAccessor = mapAccessor;
            EnvironmentInstance = environmentInstance;
            Config = new TileMapSensorConfig(size, trackPosition, detectableLayers, debug, teamId, buffer);
            _name = name;

            MObservations = new GridSpace[Config.SizeX, Config.SizeY];
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
            var map = MapAccessor.GetMap();
            var startEnd = Config.GetTrackPosition();
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
            foreach (var entityList in EnvironmentInstance.EntityMapPositions)
            {
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    var cell = MapAccessor.GetPosition(entity.Position);
                    var trackPos = Config.GetTrackPosition();

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
