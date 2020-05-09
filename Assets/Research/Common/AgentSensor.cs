using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common
{
    public class AgentSensor : ISensor
    {
        private readonly int[] _mShape;

        private readonly TopDownAgent _topdownAgent;
        private readonly float[] _mObservations;
        private readonly AgentSense[] _agentSenses;
        
        public string GetName() { return "Agent Sensor"; }
        
        public int[] GetObservationShape() { return _mShape; }
        
        public byte[] GetCompressedObservation() { return null; }
        public void Reset() { Array.Clear(_mObservations, 0, _mObservations.Length); }

        public SensorCompressionType GetCompressionType() { return SensorCompressionType.None; }
        

        public AgentSensor(TopDownAgent topdownAgent)
        {
            _topdownAgent = topdownAgent;
            _agentSenses = new[] {new AgentSense(), new AgentSense()};
            const int size = 7 * 2;
            _mShape = new[] { size };
            _mObservations = new float [ size ];
        }

        public int Write(ObservationWriter writer)
        {
            writer.AddRange(_mObservations);
            return _mObservations.Length;
        }

        public Vector2 Position
        {
            get
            {
                if (_topdownAgent && _topdownAgent.groundRb)
                {
                    return _topdownAgent.groundRb.position;
                }
                return new Vector2();
            }
        }

        public void Update()
        {
            var outputs1 = _agentSenses[0].GetPooledStats(Position);
            var outputs2 = _agentSenses[1].GetPooledStats(Position);
            var index = 0;
            foreach (var output in outputs1)
            {
                _mObservations[index++] = output;
            }
            foreach (var output in outputs2)
            {
                _mObservations[index++] = output;
            }
        }

        public void SetAgent(Character agent)
        {
            _agentSenses[0].Agent = agent;
        }
        
        public void SetOtherAgent(Character agent)
        {
            _agentSenses[1].Agent = agent;
        }
    }
}
