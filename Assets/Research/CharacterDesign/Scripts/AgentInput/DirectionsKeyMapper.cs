using System;
using System.Collections;
using System.Collections.Generic;
using Research.CharacterDesign.Scripts;
using Research.CharacterDesign.Scripts.AgentInput;
using UnityEngine;

public class DirectionsKeyMapper : VectorInput
{
    private Dictionary<Vector2, Directions> _directionsVectorMap;

    private Dictionary<Directions, Vector2> _vectorDirectionsMap;

    public Directions PrimaryDirections => GetDirection(Directions);

    void Start()
    {
        Directions = new Dictionary<Directions, KeyCode>
        {
            {Research.CharacterDesign.Scripts.Directions.Left, KeyCode.A},
            {Research.CharacterDesign.Scripts.Directions.Right, KeyCode.D },
            {Research.CharacterDesign.Scripts.Directions.Down, KeyCode.S},
            {Research.CharacterDesign.Scripts.Directions.Up, KeyCode.W }
        };
        _directionsVectorMap = new Dictionary<Vector2, Directions>
        {
            { new Vector2(-1, 0), Research.CharacterDesign.Scripts.Directions.Left },
            { new Vector2(1, 0), Research.CharacterDesign.Scripts.Directions.Right },
            { new Vector2(0, 1), Research.CharacterDesign.Scripts.Directions.Up },
            { new Vector2(0, -1), Research.CharacterDesign.Scripts.Directions.Down },
        };
        _vectorDirectionsMap = new Dictionary<Directions, Vector2>
        {
            { Research.CharacterDesign.Scripts.Directions.Left, new Vector2(-1, 0)  },
            { Research.CharacterDesign.Scripts.Directions.Right, new Vector2(1, 0) },
            { Research.CharacterDesign.Scripts.Directions.Up,  new Vector2(0, 1)},
            { Research.CharacterDesign.Scripts.Directions.Down, new Vector2(0, -1) },
            { Research.CharacterDesign.Scripts.Directions.None, new Vector2(0, 0) }
        };
    }

    public Directions GetDirectionVector(Vector2 input)
    { 
        if (_directionsVectorMap.ContainsKey(input))
        {
            return _directionsVectorMap[input];
        }
        return Research.CharacterDesign.Scripts.Directions.None;
    }

    public Vector2 GetVectorDirection(Directions direction)
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

    public Directions GetDirection(Dictionary<Directions, KeyCode> directions)
    {
        if (directions != null)
        {
            var input = GetDirection();
            return GetDirectionVector(input);
        }

        return Research.CharacterDesign.Scripts.Directions.None;
    }
}
