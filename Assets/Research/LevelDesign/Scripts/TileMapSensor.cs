using System;
using System.Globalization;
using MLAgents.Policies;
using MLAgents.Sensors;
using Research.CharacterDesign.Scripts;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensor : ISensor
    {
        private int SizeX => _size - 1;
        private int SizeY => _size - 1;

        private readonly int _size = 50;

        private readonly float[] _mObservations;
        private readonly int[] _mShape;

        private readonly bool _debug;
        
        private readonly GameObject _gameObject;

        private AiAccessor Accessor => _gameObject.GetComponentInParent<AiAccessor>();

        public void Reset() { Array.Clear(_mObservations, 0, _mObservations.Length); }
        
        public byte[] GetCompressedObservation() { return null; }
        
        private GridSpace[,] GridSpaces => Accessor == null ? null : Accessor.Map;

        public TileMapSensor(GameObject gameObject, bool debug=false)
        {
            _gameObject = gameObject;
            _debug = debug;
            var obsSize = SizeX * SizeY;
            
            _mShape = new[] { obsSize };
            _mObservations = new float[obsSize];
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

        private void OutputDebug()
        {
            var output = "";
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    var value = _mObservations[x + (y * SizeX)];
                    var str = value > 0 ? value.ToString(CultureInfo.InvariantCulture) : " ";
                    output += str;
                }

                output += "\n";
            }
            Debug.Log(output);
        }

        public int Write(ObservationWriter writer)
        {
            if (_debug)
            {
                OutputDebug();
            }
            writer.AddRange(_mObservations);
            return _mObservations.Length;
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                Array.Clear(_mObservations, 0, _mObservations.Length);
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        _mObservations[x + (y * SizeX)] = (int)GridSpaces[x, y];
                    }
                }
                
                foreach (var (agent, pos) in Accessor.AgentPosition)
                {
                    var agentType = GetAgentType(agent);
                    _mObservations[pos.x + (pos.y * SizeX)] = (float) agentType;
                }
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
