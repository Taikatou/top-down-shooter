using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts;
using Unity.MLAgents.Sensors;

namespace Research.Common
{
    public class AgentSensorComponent : SensorComponent
    {
        [NonSerialized] private AgentSensor _agentSensor;

        public override ISensor CreateSensor()
        {
            var position = GetComponent<TopDownAgent>();
            _agentSensor = new AgentSensor(position);
            return _agentSensor;
        }

        public override int[] GetObservationShape()
        {
            return _agentSensor.GetObservationShape();
        }

        public void SetAgents(Character agent, Character otherAgent)
        {
            _agentSensor.SetAgent(agent);
            _agentSensor.SetOtherAgent(otherAgent);
        }
    }
}
