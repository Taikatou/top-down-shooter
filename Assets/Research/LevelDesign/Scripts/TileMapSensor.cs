using System;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
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
        
        private AiAccessor Accessor
        {
            get
            {
                if (_gameObject != null)
                {
                    return _gameObject.GetComponentInParent<AiAccessor>();;
                }
                Debug.Log("Invalid Accessor");
                return null;
            }
        }

        private GridSpace[,] GridSpaces
        {
            get
            {
                if (Accessor != null)
                {
                    return Accessor.Map;
                }
                Debug.Log("Invalid Accessor");
                return null;
            }
        }

        private Dictionary<GridSpace, int> GetGridSpaceValues()
        {
            return new Dictionary<GridSpace, int>
            {
                { GridSpace.Floor, 0 },
                { GridSpace.Self, 1 },
                { GridSpace.Wall, 2},
                { GridSpace.OtherTeam, 3 }
            };
        }
        
        public TileMapSensor(GameObject gameObject, bool debug)
        {
            _gameObject = gameObject;
            _debug = debug;

            var detectable = GetGridSpaceValues().Count;
            _mShape = new[] { SizeX, SizeY, detectable };
            _mObservations = new GridSpace[SizeX, SizeY];
        }

        public int[] GetObservationShape()
        {
            return _mShape;
        }

        private GridSpace GetAgentType(TopDownAgent agent)
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

        private void OutputDebug(GridSpace [,] debugGrid)
        {
            var output = "";
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    var value = debugGrid[x, y];
                    output += value;
                }
                output += "\n";
            }
            Debug.Log(output);
        }

        public int Write(ObservationWriter writer)
        {
            var typesOfGrid = GetGridSpaceValues();

            foreach (var pair in typesOfGrid)
            {
                var gridInt = pair.Value;
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        var present = _mObservations[x, y] == pair.Key? 1.0f: 0.0f;
                        writer[x, y, gridInt] = present;
                    }
                }   
            }

            return _mShape[0] * _mShape[1] * _mShape[2];
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                Array.Copy(GridSpaces, _mObservations, SizeX*SizeY);
                
                foreach (var (agent, pos) in Accessor.AgentPosition)
                {
                    var agentType = GetAgentType(agent);
                    _mObservations[pos.x, pos.y] = agentType;
                }
                
                if (_debug)
                {
                    OutputDebug(_mObservations);
                }
            }
            else
            {
                Debug.Log("Invalid gridarray");
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
