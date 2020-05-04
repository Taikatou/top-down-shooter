using System;
using MLAgents.Policies;
using MLAgents.Sensors;
using Research.CharacterDesign.Scripts;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensor : ISensor
    {
        private int _sizeX = 50;
        private int _sizeY = 50;

        float[] m_Observations;
        int[] m_Shape;
        
        private readonly GameObject _gameObject;

        private AiAccessor Accessor => _gameObject.GetComponentInParent<AiAccessor>();

        public void Reset() { }
        
        public byte[] GetCompressedObservation() { return null; }
        
        private GridSpace[,] GridSpaces => Accessor == null ? null : Accessor.Map;
        
        private void SetNumObservations(int numObservations)
        {
            m_Shape = new[] { numObservations };
            m_Observations = new float[numObservations];
        }

        public TileMapSensor(GameObject gameObject, int sizeX, int sizeY)
        {
            _gameObject = gameObject;
            var obsSize = _sizeX * _sizeY;
            SetNumObservations(obsSize);

            _sizeX = sizeX;
            _sizeY = sizeX;
        }

        public int[] GetObservationShape()
        {
            return m_Shape;
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

        public int Write(ObservationWriter writer)
        {
            writer.AddRange(m_Observations);
            return m_Observations.Length;
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                Array.Clear(m_Observations, 0, m_Observations.Length);
                for (var x = 0; x < _sizeX; x++)
                {
                    for (var y = 0; y < _sizeY; y++)
                    {
                        m_Observations[x + (y * _sizeX)] = (int)GridSpaces[x, y];
                    }
                }
                
                foreach (var pairs in Accessor.AgentPosition)
                {
                    var agentType = GetAgentType(pairs.Item1);
                    var pos = pairs.Item2;
                    m_Observations[pos.x + (pos.y * _sizeX)] = (float) agentType;
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
