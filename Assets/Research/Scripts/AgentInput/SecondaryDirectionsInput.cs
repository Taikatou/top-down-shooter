using System.Collections.Generic;
using UnityEngine;

namespace Research.Scripts.AgentInput
{
    public class SecondaryDirectionsInput : VectorInput
    {
        public Vector2 SecondaryDirection => GetDirection();
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
