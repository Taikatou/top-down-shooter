using System.Collections.Generic;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
{
    public class VectorInput : MonoBehaviour
    {
        protected Dictionary<Directions, KeyCode> Directions;

        protected virtual int NegativeValue => -1;

        protected virtual int PositiveValue => 1;

        protected Vector2 GetDirection()
        {
            if (Directions != null)
            {
                var x = GetInput(Directions[Scripts.Directions.Left], Directions[Scripts.Directions.Right]);
                var y = GetInput(Directions[Scripts.Directions.Down], Directions[Scripts.Directions.Up]);
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
