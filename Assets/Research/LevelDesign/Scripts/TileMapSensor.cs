using System;
using System.Collections.Generic;
using Research.Common.MapSensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensor : ISensor
    {
        private int SizeX => _size - 1;
        private int SizeY => _size - 1;

        private readonly int _size = 50;

        private readonly GridSpace[,] _mObservations;
        private readonly int[] _mShape;

        private readonly bool _debug;
        
        private readonly GameObject _learningEnvironment;

        private int _teamId;
        
        public void Reset() { Array.Clear(_mObservations, 0, SizeX*SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }


        private MapAccessor MapAccessor
        {
            get
            {
                if (_learningEnvironment != null)
                {
                    var mapAccessor = _learningEnvironment.GetComponentInChildren<MapAccessor>();
                    return mapAccessor;
                }
                return null;
            }
        }

        private GridSpace[,] GridSpaces
        {
            get
            {
                if (MapAccessor != null)
                {
                    return MapAccessor.Map;
                }
                return null;
            }
        }

        private readonly Dictionary<GridSpace, int> _gridSpaceValues = new Dictionary<GridSpace, int>
            {
                { GridSpace.Floor, 0 },
                { GridSpace.Wall, 1 },
                { GridSpace.Self, 2 },
                { GridSpace.Team1, 3 },
                { GridSpace.Team2, 4 }
            };

        public TileMapSensor(GameObject learningEnvironment, int teamId, bool debug)
        {
            _learningEnvironment = learningEnvironment;
            _debug = debug;
            _teamId = teamId;

            var detectable = _gridSpaceValues.Count;
            _mShape = new[] { SizeX, SizeY, detectable };
            _mObservations = new GridSpace[SizeX, SizeY];
        }

        public int[] GetObservationShape()
        {
            return _mShape;
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
            var typesOfGrid = _gridSpaceValues;

            foreach (var pair in typesOfGrid)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        var isKey = _mObservations[x, y] == pair.Key;
                        var present = isKey? 1.0f: 0.0f;
                        writer[x, y, gridInt] = present;
                    }
                }   
            }
            var outputSize = _mShape[0] * _mShape[1] * _mShape[2];
            return outputSize;
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        _mObservations[x, y] = GridSpaces[x, y];
                    }
                }

                var entities = _learningEnvironment.GetComponentsInChildren<EntityMapPosition>();
                
                foreach (var entity in entities)
                {
                    var position = entity.transform.position;
                    var cell = MapAccessor.GetPosition(position);
                    _mObservations[cell.x, cell.y] = entity.GetType(_teamId);
                }
                
                if (_debug)
                {
                    OutputDebugMap(_mObservations);
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
            return "TopDown Sensor";
        }
    }
}
