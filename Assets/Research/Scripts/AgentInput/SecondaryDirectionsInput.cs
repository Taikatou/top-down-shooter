using System;
using System.Collections.Generic;
using UnityEngine;

namespace Research.Scripts.AgentInput
{
    public class SecondaryDirectionsInput : VectorInput
    {
        public Vector2 SecondaryDirection => GetDirection();
        
        protected override int NegativeValue => 1;

        protected override int PositiveValue => 2;
        
        void Start()
        {
            Directions = new Dictionary<Directions, KeyCode>
            {
                {Research.Scripts.Directions.Left, KeyCode.LeftArrow},
                {Research.Scripts.Directions.Right, KeyCode.RightArrow },
                {Research.Scripts.Directions.Down, KeyCode.DownArrow },
                {Research.Scripts.Directions.Up, KeyCode.UpArrow }
            };
        }
    }
}
