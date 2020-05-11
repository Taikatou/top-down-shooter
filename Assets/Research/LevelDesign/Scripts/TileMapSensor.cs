using System;
using System.Collections.Generic;
using Research.Common;
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
        
        private readonly GameObject _gameObject;

        public void Reset() { Array.Clear(_mObservations, 0, SizeX*SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }

        private MapAccessor MapAccessor
        {
            get
            {
                if (_gameObject != null)
                {
                    var mapAccessor = _gameObject.GetComponentInParent<MapAccessor>();
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
                { GridSpace.OtherTeam, 3 }
            };

        public TileMapSensor(GameObject gameObject, bool debug)
        {
            _gameObject = gameObject;
            _debug = debug;

            var detectable = _gridSpaceValues.Count;
            _mShape = new[] { SizeX, SizeY, detectable };
            _mObservations = new GridSpace[SizeX, SizeY];
        }

        public int[] GetObservationShape()
        {
            return _mShape;
        }

        private GridSpace GetAgentType(AgentPosition agent)
        {
            if (agent.gameObject == _gameObject)
            {
                return GridSpace.Self;
            }
            var behaviour = _gameObject.GetComponent<BehaviorParameters>();
            var otherBehaviour = agent.GetComponent<BehaviorParameters>();
            var isTeam = behaviour.TeamId == otherBehaviour.TeamId;

            return isTeam ? GridSpace.OurTeam : GridSpace.OtherTeam;
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
            Debug.Log(_mShape[2]);
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

                foreach (var (agent, pos) in AiAccessor)
                {
                    var agentType = GetAgentType(agent);
                    _mObservations[pos.x, pos.y] = agentType;
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
