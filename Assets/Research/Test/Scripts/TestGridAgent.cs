using System.Collections.Generic;
using Research.CharacterDesign.Scripts.AgentInput;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Test.Scripts
{
    public class TestGridAgent : Agent
    {
        public TestGridController controller;
        public DirectionsKeyMapper directionsKeyMapper;

        public TestPickup coinPickup;
        
        private Vector3 _mTransform;
        
        public float punishValue = -0.0005f;
        public override void Initialize()
        {
            _mTransform = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

        public override void CollectObservations(VectorSensor sensor)
        {

        }

        public override void Heuristic(float[] actionsOut)
        {
            var index = 0;
            actionsOut[index++] = (int) directionsKeyMapper.PrimaryDirections;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            AddReward(punishValue);

            var counter = 0;
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[counter++]);
            controller.Input = primaryDirection;
        }

        public override void OnEpisodeBegin()
        {
            transform.position = _mTransform;
            controller.ResetMovePoint(_mTransform);
            
            coinPickup.ResetPosition();
        }
    }
}
