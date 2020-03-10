using System;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

namespace Research.Scripts
{
    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        private enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> _directions;

        private Dictionary<Directions, KeyCode> _secondaryDirections;
        public override void InitializeAgent()
        {
            base.InitializeAgent();

            _directions = new Dictionary<Directions, KeyCode>()
            {
                {Directions.Left, KeyCode.A},
                {Directions.Right, KeyCode.D },
                {Directions.Down, KeyCode.S},
                {Directions.Up, KeyCode.W }
            };
            _secondaryDirections = new Dictionary<Directions, KeyCode>()
            {
                {Directions.Left, KeyCode.LeftArrow},
                {Directions.Right, KeyCode.RightArrow },
                {Directions.Down, KeyCode.DownArrow },
                {Directions.Up, KeyCode.UpArrow }
            };
        }

        private int GetDecision(float input)
        {
            var output = 0;
            switch (Mathf.FloorToInt(input))
            {
                case 1:
                    // Left or Down
                    output = -1;
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

            // inputManager.SetAIPrimaryMovement(xInput, yInput);
        }

        private int GetInput(KeyCode negativeKey, KeyCode positiveKey)
        {
            var negative = Input.GetKey(negativeKey);
            var positive = Input.GetKey(positiveKey);
            if (negative ^ positive)
            {
                return negative ? 1 : 2;
            }

            return 0;
        }

        public override float[] Heuristic()
        {
            var direction = 0.0f;


            if (_directions != null)
            {
                var x = GetInput(_directions[Directions.Left], _directions[Directions.Right]);
                var y = GetInput(_directions[Directions.Down], _directions[Directions.Up]);
                
            }

            var output = new[] { direction };

            return output;
        }
    }
}
