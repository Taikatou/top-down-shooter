using System.Collections.Generic;
using Research.Common;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
{
    public class DirectionsKeyMapper : VectorInput
    {
        private Dictionary<Vector2, Directions> _directionsVectorMap;

        private Dictionary<Directions, Vector2> _vectorDirectionsMap;

        public Directions PrimaryDirections
        {
            get
            {
                var input = GetDirection();
                return GetDirectionVector(input);
            }
        }

        void Start()
        {
            InputDirections = new Dictionary<Directions, KeyCode>
            {
                {Directions.Left, KeyCode.A},
                {Directions.Right, KeyCode.D },
                {Directions.Down, KeyCode.S},
                {Directions.Up, KeyCode.W }
            };
            _directionsVectorMap = new Dictionary<Vector2, Directions>
            {
                { new Vector2(-1, 0), Directions.Left },
                { new Vector2(1, 0), Directions.Right },
                { new Vector2(0, 1), Directions.Up },
                { new Vector2(0, -1), Directions.Down },
            };
            _vectorDirectionsMap = new Dictionary<Directions, Vector2>
            {
                { Directions.Left, new Vector2(-1, 0)  },
                { Directions.Right, new Vector2(1, 0) },
                { Directions.Up,  new Vector2(0, 1)},
                { Directions.Down, new Vector2(0, -1) },
                { Directions.None, new Vector2(0, 0) }
            };
        }

        public Directions GetDirectionVector(Vector2 input)
        { 
            if (_directionsVectorMap.ContainsKey(input))
            {
                return _directionsVectorMap[input];
            }
            return Directions.None;
        }

        private Vector2 GetVectorDirection(Directions direction)
        {
            if (_vectorDirectionsMap != null)
            {
                return _vectorDirectionsMap[direction];
            }
            return new Vector2();
        }

        public Vector2 GetVectorDirection(float direction)
        {
            return GetVectorDirection((Directions)direction);
        }
    }
}
