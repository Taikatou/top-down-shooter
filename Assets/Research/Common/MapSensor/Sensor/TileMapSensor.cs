using System;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
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

        public string GetName() { return Config.Name; }
        
        public SensorCompressionType GetCompressionType() { return SensorCompressionType.None; }

        public void Reset() { Array.Clear(MObservations, 0, Config.SizeX*Config.SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }
        
        public int[] GetObservationShape() { return MShape; }
        
        private GridSpace[,] GridSpaces => MapAccessor? MapAccessor.Map: null;
        
        TrackPosition StartEnd => Config.GetTrackPositionPosition(Position);
        
        public List<TopDownAgent> EntityList { get; set; }
        public Vector3Int Position { get; set; }
        public MapAccessor MapAccessor { get; set; }
        
        protected abstract int WriteObservations(ObservationWriter writer);

        protected TileMapSensor(string name, int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers)
        {
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
                var startEnd = StartEnd;
                UpdateMap(startEnd.StartPos.x, startEnd.StartPos.y, 
                    startEnd.EndPos.x, startEnd.EndPos.y);
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
                UpdateMapEntityPositions();
            }
            else
            {
                Debug.Log("Invalid grid array");
            }
        }

        public void UpdateMapEntityPositions()
        {
            foreach (var entity in EntityList)
            {
                if (entity.gameObject.activeInHierarchy)
                {
                    var entityMap = entity.GetComponentInParent<EntityMapPosition>();

                    var position = entity.transform.position;
                    var cell = MapAccessor.GetPosition(position);
                    
                    var gridType = entityMap.GetGridSpaceType();
                    if (Config.GridSpaceValues.ContainsKey(gridType))
                    {
                        UpdateEntityPosition(cell, gridType);
                    }   
                }
            }
        }

        private void UpdateEntityPosition(Vector3Int cell, GridSpace gridType)
        {
            if (Config.TrackPosition)
            {
                var startEnd = StartEnd;
                
                var validX = cell.x >= startEnd.StartPos.x && cell.x < startEnd.EndPos.x;
                var validY = cell.y >= startEnd.StartPos.y && cell.y < startEnd.EndPos.y;
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
