using System.Collections.Generic;
using Research.Common;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
{
    public class SecondaryDirectionsInput : VectorInput
    {
        public Vector2 SecondaryDirection => GetDirection();
        
        protected override int NegativeValue => 1;

        protected override int PositiveValue => 2;
        
        private void Start()
        {
            InputDirections = new Dictionary<EDirections, KeyCode>
            {
                {EDirections.Left, KeyCode.LeftArrow},
                {EDirections.Right, KeyCode.RightArrow },
                {EDirections.Down, KeyCode.DownArrow },
                {EDirections.Up, KeyCode.UpArrow }
            };
        }
    }
}
