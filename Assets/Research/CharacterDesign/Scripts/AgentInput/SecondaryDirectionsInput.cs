using System;
using System.Collections.Generic;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
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
                {Research.CharacterDesign.Scripts.Directions.Left, KeyCode.LeftArrow},
                {Research.CharacterDesign.Scripts.Directions.Right, KeyCode.RightArrow },
                {Research.CharacterDesign.Scripts.Directions.Down, KeyCode.DownArrow },
                {Research.CharacterDesign.Scripts.Directions.Up, KeyCode.UpArrow }
            };
        }
    }
}
