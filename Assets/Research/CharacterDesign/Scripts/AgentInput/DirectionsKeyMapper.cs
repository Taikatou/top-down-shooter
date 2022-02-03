using System.Collections.Generic;
using Research.Common;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.AgentInput
{
    public class DirectionsKeyMapper : VectorInput
    {
        private Dictionary<Vector2, EDirections> _directionsVectorMap;

        private Dictionary<EDirections, Vector2> _vectorDirectionsMap;

        public EDirections PrimaryDirections
        {
            get
            {
                var input = GetDirection();
                return GetDirectionVector(input);
            }
        }

        private void Start()
        {
            InputDirections = new Dictionary<EDirections, KeyCode>
            {
                { EDirections.Left, KeyCode.A },
                { EDirections.Right, KeyCode.D },
                { EDirections.Down, KeyCode.S },
                { EDirections.Up, KeyCode.W }
            };
            _directionsVectorMap = new Dictionary<Vector2, EDirections>
            {
                { new Vector2(-1, 0), EDirections.Left },
                { new Vector2(1, 0), EDirections.Right },
                { new Vector2(0, 1), EDirections.Up },
                { new Vector2(0, -1), EDirections.Down },
            };
            _vectorDirectionsMap = new Dictionary<EDirections, Vector2>
            {
                { EDirections.Left, new Vector2(-1, 0)  },
                { EDirections.Right, new Vector2(1, 0) },
                { EDirections.Up,  new Vector2(0, 1)},
                { EDirections.Down, new Vector2(0, -1) },
                { EDirections.None, new Vector2(0, 0) }
            };
        }

        private EDirections GetDirectionVector(Vector2 input)
        { 
            if (_directionsVectorMap != null && _directionsVectorMap.ContainsKey(input))
            {
                return _directionsVectorMap[input];
            }
            return EDirections.None;
        }

        public Vector2 GetVectorDirection(EDirections direction)
        {
            if (_vectorDirectionsMap != null)
            {
                return _vectorDirectionsMap[direction];
            }
            return new Vector2();
        }

        public Vector2 GetVectorDirection(int direction)
        {
            return GetVectorDirection((EDirections)direction);
        }
    }
}
