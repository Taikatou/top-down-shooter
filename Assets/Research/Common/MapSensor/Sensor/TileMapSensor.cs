using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public abstract class TileMapSensor : ISensor
    {
        public string Name;
        
        public readonly TileMapSensorConfig Config;
        protected readonly GridSpace[,] MObservations;
        protected abstract int[] MShape { get; }

        public string GetName() => Name;
        
        public SensorCompressionType GetCompressionType() { return SensorCompressionType.None; }

        public void Reset() { Array.Clear(MObservations, 0, Config.SizeX*Config.SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }
        
        public int[] GetObservationShape() { return MShape; }

        private readonly GetEnvironmentMapPositions _environmentInstance;

        private readonly MapAccessor _mapAccessor;

        protected abstract int WriteObservations(ObservationWriter writer);

        protected TileMapSensor(string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers,
            MapAccessor mapAccessor, GetEnvironmentMapPositions environmentInstance, int teamId, bool buffer)
        {
            _mapAccessor = mapAccessor;
            _environmentInstance = environmentInstance;
            Config = new TileMapSensorConfig(size, trackPosition, detectableLayers, debug, teamId, buffer);
            Name = name;
            
            MObservations = new GridSpace[Config.SizeX, Config.SizeY];
        }

        public static void OutputDebugMap(GridSpace [,] debugGrid)
        {
            var roomWidth = debugGrid.GetUpperBound(0);
            var roomHeight = debugGrid.GetUpperBound(1);
            var output = "Output log \n";
            for (var y = roomHeight - 1; y >= 0; y--)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    output += (int) debugGrid[x, y];
                }
                output += "\n";
            }
            Debug.Log(output);
        }

        public int Write(ObservationWriter writer)
        {
            using (TimerStack.Instance.Scoped("TileMapSensor.WriteToTensor"))
            {
                return WriteObservations(writer);
            }
        }

        private void UpdateMap()
        {
            var startEnd = Config.GetTrackPosition();
            for (var y = startEnd.StartPos.y; y < startEnd.EndPos.y; y++)
            {
                for (var x = startEnd.StartPos.x; x < startEnd.EndPos.x; x++)
                {
                    var value =_mapAccessor.Map[x, y];
                    MObservations[x, y] = value;
                }
            }
        }

        public void Update()
        {
            UpdateMap();
            UpdateMapEntityPositions();
        }

        private void UpdateMapEntityPositions()
        {
            foreach (var entityList in  _environmentInstance.EntityMapPositions)
            {
                foreach (var entity in entityList.GetGridSpaceType(Config.TeamId))
                {
                    var cell = _mapAccessor.GetPosition(entity.Position);
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
