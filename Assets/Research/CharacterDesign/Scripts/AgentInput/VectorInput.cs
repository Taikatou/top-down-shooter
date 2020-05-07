using System.Collections.Generic;
using Research.Common;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
{
    public class VectorInput : MonoBehaviour
    {
        protected Dictionary<Directions, KeyCode> InputDirections;

        protected virtual int NegativeValue => -1;

        protected virtual int PositiveValue => 1;

        protected Vector2 GetDirection()
        {
            if (InputDirections != null)
            {
                var x = GetInput(InputDirections[Directions.Left], InputDirections[Directions.Right]);
                var y = GetInput(InputDirections[Directions.Down], InputDirections[Directions.Up]);
                return new Vector2(x, y);
            }

            return new Vector2();
        }
        
        private int GetInput(KeyCode negativeKey, KeyCode positiveKey)
        {
            var negative = Input.GetKey(negativeKey);
            var positive = Input.GetKey(positiveKey);
            if (negative ^ positive)
            {
                return negative ? NegativeValue : PositiveValue;
            }

            return 0;
        }
    }
}
