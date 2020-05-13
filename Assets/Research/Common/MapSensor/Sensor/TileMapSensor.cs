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
        protected int SizeX => Config.Size - 1;
        protected int SizeY => Config.Size - 1;

        public void Reset() { Array.Clear(MObservations, 0, SizeX*SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }
        
        public int[] GetObservationShape() { return MShape; }
        
        private GridSpace[,] GridSpaces => MapAccessor? MapAccessor.Map: null;

        private MapAccessor MapAccessor =>
            _learningEnvironment ? _learningEnvironment.GetComponentInChildren<MapAccessor>() : null;
        
        protected abstract int WriteObservations(ObservationWriter writer);

        protected TileMapSensor(GameObject learningEnvironment, string name,
            int size, bool trackPosition, bool debug, IEnumerable<GridSpace> detectableLayers)
        {
            _learningEnvironment = learningEnvironment;
            Config = new TileMapSensorConfig(size, trackPosition, name, detectableLayers, debug);

            var detectable = Config.GridSpaceValues.Count;
            MShape = new[] { SizeX, SizeY, detectable };
            MObservations = new GridSpace[SizeX, SizeY];
        }

        private static void OutputDebugMap(GridSpace [,] debugGrid)
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
        
        public void Update()
        {
            if (GridSpaces != null)
            {
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        MObservations[x, y] = GridSpaces[x, y];
                    }
                }

                var entities = _learningEnvironment.GetComponentsInChildren<EntityMapPosition>();
                
                foreach (var entity in entities)
                {
                    var position = entity.transform.position;
                    var cell = MapAccessor.GetPosition(position);
                    MObservations[cell.x, cell.y] = entity.GetGridSpaceType();
                }
                
                if (Config.Debug)
                {
                    OutputDebugMap(MObservations);
                }
                else
                {
                    Debug.Log("No debug trace");
                }
            }
            else
            {
                Debug.Log("Invalid grid array");
            }
        }

        public SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }

        public string GetName()
        {
            return Config.Name;
        }
    }
}
