using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Test.Scripts
{
    public class TestGridAgent : Agent
    {
        public Transform movePosition;
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
            
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            AddReward(punishValue);
        }

        public override void OnEpisodeBegin()
        {
            transform.position = _mTransform;
            movePosition.position = _mTransform;
        }
    }
}
