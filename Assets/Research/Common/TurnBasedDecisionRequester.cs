using System.Collections.Generic;
using Research.Test.Scripts;
using Unity.MLAgents;
using UnityEngine;

namespace Research.Common
{
    public class TurnBasedDecisionRequester : MonoBehaviour
    {
        private float _timer;

        public List<TestGridAgent> mAgents;

        internal void Awake()
        {
            Academy.Instance.AgentPreStep += MakeRequests;
            _timer = 0.0f;
        }

        void OnDestroy()
        {
            if (Academy.IsInitialized)
            {
                Academy.Instance.AgentPreStep -= MakeRequests;
            }
        }
        private void MakeRequests(int academyStepCount)
        {
            _timer -= Time.deltaTime;
            var canMove = _timer <= 0.0f;
            if (canMove)
            {
                foreach(var mAgent in mAgents)
                {
                    mAgent.CompleteMovement();
                    mAgent.RequestDecision();
                }
                _timer = 0.1f;
            }
        }
    }
}
