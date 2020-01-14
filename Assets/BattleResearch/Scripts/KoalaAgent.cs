using MLAgents;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class KoalaAgent : Agent, IInput
    {
        private float xAxis = 0.0f;

        private float yAxis = 0.0f;

        public float XAxis => xAxis;
        public float YAxis => yAxis;

        private float GetDecision(float input)
        {
            var output = 0.0f;
            switch (input)
            {
                case 1:
                    // Left or Down
                    output = -1.0f;
                    break;
                case 2:
                    // Right or Up
                    output = 1;
                    break;
            }
            return output;
        }

        public override void AgentAction(float[] vectorAction)
        {
            var xInput = Mathf.FloorToInt(vectorAction[0]);
            var yInput = Mathf.FloorToInt(vectorAction[1]);
            xAxis = GetDecision(xInput);
            yAxis = GetDecision(yInput);
        }
        
        public override float[] Heuristic()
        {
            var x = 0;
            var y = 0;
            if (Input.GetKey(KeyCode.D))
            {
                x = 2;
            }
            if (Input.GetKey(KeyCode.W))
            {
                y = 2;
            }
            if (Input.GetKey(KeyCode.A))
            {
                x = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                y = 1;
            }
            return new float[] { x, y };
        }
    }
}
