using System;
using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    [RequireComponent(typeof(MlAgentInput))]
    public class TopDownAgent : Agent
    {
        public MlAgentInput AgentInput => GetComponent<MlAgentInput>();

        private enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> _directions;

        private Dictionary<Directions, KeyCode> _secondaryDirections;

        private void Start()
        {
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
            var xInput = GetDecision(vectorAction[0]);
            var yInput = GetDecision(vectorAction[1]);

            AgentInput.PrimaryInput = new Vector2(xInput, yInput);

            var secondaryXInput = GetDecision(vectorAction[2]);
            var secondaryYInput = GetDecision(vectorAction[3]);
            var secondary = new Vector2(secondaryXInput, secondaryYInput);

            AgentInput.SecondaryInput = secondary;

            var shootButtonDown = Convert.ToBoolean(vectorAction[4]);
            
            AgentInput.SetButton(shootButtonDown);
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
            var x = 0.0f;
            var y = 0.0f;
            var secondaryX = 0.0f;
            var secondaryY = 0.0f;

            if (_directions!=null && _secondaryDirections!=null)
            {
                x = GetInput(_directions[Directions.Left], _directions[Directions.Right]);
                y = GetInput(_directions[Directions.Down], _directions[Directions.Up]);

                secondaryX = GetInput(_secondaryDirections[Directions.Left], _secondaryDirections[Directions.Right]);
                secondaryY = GetInput(_secondaryDirections[Directions.Down], _secondaryDirections[Directions.Up]);
            }

            var buttonState = Input.GetKey(KeyCode.KeypadEnter);
            var buttonInput = Convert.ToSingle(buttonState);
            
            return new [] { x, y, secondaryX, secondaryY, buttonInput};
        }

        private void ConfigureAgent(int config)
        {
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.MlCuriculum, null);
        }
    }
}
