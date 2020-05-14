using System;
using System.Collections.Generic;
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
        protected readonly TileMapSensorConfig Config;
        protected readonly GridSpace[,] MObservations;
        protected readonly int[] MShape;
        
        private readonly GameObject _learningEnvironment;
        
        private int HalfX => Config.Size / 2;
        private int HalfY => Config.Size / 2;

        private int StartX => Position.x - HalfX;
        private int StartY => Position.y - HalfY;

        private int EndX => Position.x + HalfX;
        private int EndY => Position.y + HalfY;
        
        public Vector3Int Position { get; set; }
        
        public string GetName() { return Config.Name; }
        
        public SensorCompressionType GetCompressionType() { return SensorCompressionType.None; }

        public void Reset() { Array.Clear(MObservations, 0, Config.SizeX*Config.SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }
        
        public int[] GetObservationShape() { return MShape; }
        
        private GridSpace[,] GridSpaces => MapAccessor? MapAccessor.Map: null;

        public MapAccessor MapAccessor =>
            _learningEnvironment ? _learningEnvironment.GetComponentInChildren<MapAccessor>() : null;
        
        protected abstract int WriteObservations(ObservationWriter writer);

        protected TileMapSensor(GameObject learningEnvironment, string name,
            int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers)
        {
            _learningEnvironment = learningEnvironment;
            Config = new TileMapSensorConfig(size, trackPosition, name, detectableLayers, debug);

            var detectable = Config.GridSpaceValues.Count;
            MShape = new[] { Config.SizeX, Config.SizeY, detectable };
            MObservations = new GridSpace[Config.SizeX, Config.SizeY];
        }

        protected static void OutputDebugMap(GridSpace [,] debugGrid)
        {
            var roomWidth = debugGrid.GetUpperBound(0);
            var roomHeight = debugGrid.GetUpperBound(1);
            var output = "Output log \n";
            for (var y = roomHeight; y >= 0; y--)
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

        private void UpdateMap(int startX, int startY, int endX, int endY)
        {
            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    if (x >= 0 && x < Config.SizeX && y >= 0 && y < Config.SizeY)
                    {
                        MObservations[x, y] = GridSpaces[x, y];
                    }
                }
            }
        }

        private void UpdateMap()
        {
            if (Config.TrackPosition)
            {
                UpdateMap(StartX, StartY, EndX, EndY);
            }
            else
            {
                UpdateMap(0, 0, Config.SizeX, Config.SizeY); 
            }
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                UpdateMap();

                var entities = _learningEnvironment.GetComponentsInChildren<EntityMapPosition>();
                
                foreach (var entity in entities)
                {
                    var position = entity.transform.position;
                    var cell = MapAccessor.GetPosition(position);
                    
                    var gridType = entity.GetGridSpaceType();
                    if (Config.GridSpaceValues.ContainsKey(gridType))
                    {
                        UpdateEntityPosition(cell, gridType);
                    }
                }
            }
            else
            {
                Debug.Log("Invalid grid array");
            }
        }

        private void UpdateEntityPosition(Vector3Int cell, GridSpace gridType)
        {
            if (Config.TrackPosition)
            {
                var validX = cell.x >= StartX && cell.x < EndX;
                var validY = cell.y >= StartY && cell.y < EndY;
                if (validX && validY)
                {
                    var x = cell.x - Position.x;
                    var y = cell.y - Position.y;
                    MObservations[x, y] = gridType;
                }
            }
            else
            {
                MObservations[cell.x, cell.y] = gridType;
            }
        }
    }
}
