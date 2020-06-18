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
            InputDirections = new Dictionary<Directions, KeyCode>
            {
                {Directions.Left, KeyCode.LeftArrow},
                {Directions.Right, KeyCode.RightArrow },
                {Directions.Down, KeyCode.DownArrow },
                {Directions.Up, KeyCode.UpArrow }
            };
        }
    }
}
