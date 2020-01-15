using System.Collections.Generic;
using MLAgents;
using UnityEngine;

namespace BattleResearch.Scripts
{
    [RequireComponent(typeof(MlAgentInput))]
    public class KoalaAgent : Agent
    {
        private MlAgentInput agentInput => GetComponent<MlAgentInput>();

        public enum Directions { Left, Right, Up, Down }

        private Dictionary<Directions, KeyCode> directions;

        private Dictionary<Directions, KeyCode> secondaryDirections;

        private void Start()
        {
            directions = new Dictionary<Directions, KeyCode>()
            {
                {Directions.Left, KeyCode.A},
                {Directions.Right, KeyCode.D },
                {Directions.Down, KeyCode.S},
                {Directions.Up, KeyCode.W }
            };
            secondaryDirections = new Dictionary<Directions, KeyCode>()
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

            agentInput.PrimaryInput = new Vector2(xInput, yInput);

            var secondaryXInput = GetDecision(vectorAction[2]);
            var secondaryYInput = GetDecision(vectorAction[3]);
            var secondary = new Vector2(secondaryXInput, secondaryYInput);

            agentInput.SecondaryInput = secondary;
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

            if (directions!=null && secondaryDirections!=null)
            {
                x = GetInput(directions[Directions.Left], directions[Directions.Right]);
                y = GetInput(directions[Directions.Down], directions[Directions.Up]);

                secondaryX = GetInput(secondaryDirections[Directions.Left], secondaryDirections[Directions.Right]);
                secondaryY = GetInput(secondaryDirections[Directions.Down], secondaryDirections[Directions.Up]);
            }
            return new float[] { x, y, secondaryX, secondaryY };
        }
    }
}
