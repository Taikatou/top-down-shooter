using System;
using System.Collections;
using System.Collections.Generic;
using Research.Scripts;
using UnityEngine;

public class DirectionsKeyMapper : MonoBehaviour
{
    private Dictionary<Directions, KeyCode> _primaryDirections;

    private Dictionary<Directions, KeyCode> _secondaryDirections;

    private Dictionary<Vector2, Directions> _directionsVectorMap;

    private Dictionary<Directions, Vector2> _vectorDirectionsMap;

    public Directions PrimaryDirections => GetDirection(_primaryDirections);

    public Directions SecondaryDirections => GetDirection(_secondaryDirections);

    void Start()
    {
        _primaryDirections = new Dictionary<Directions, KeyCode>
        {
            {Directions.Left, KeyCode.A},
            {Directions.Right, KeyCode.D },
            {Directions.Down, KeyCode.S},
            {Directions.Up, KeyCode.W }
        };
        _secondaryDirections = new Dictionary<Directions, KeyCode>
        {
            {Directions.Left, KeyCode.LeftArrow},
            {Directions.Right, KeyCode.RightArrow },
            {Directions.Down, KeyCode.DownArrow },
            {Directions.Up, KeyCode.UpArrow }
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

    private int GetInput(KeyCode negativeKey, KeyCode positiveKey)
    {
        var negative = Input.GetKey(negativeKey);
        var positive = Input.GetKey(positiveKey);
        if (negative ^ positive)
        {
            return negative ? -1 : 1;
        }

        return 0;
    }

    public Directions GetDirection(Dictionary<Directions, KeyCode> directions)
    {
        if (directions != null)
        {
            if (directions.ContainsKey(Directions.Left) && directions.ContainsKey(Directions.Right) &&
                directions.ContainsKey(Directions.Down) && directions.ContainsKey(Directions.Up))
            {
                var x = GetInput(directions[Directions.Left], directions[Directions.Right]);
                var y = GetInput(directions[Directions.Down], directions[Directions.Up]);

                var input = new Vector2(x, y);
                return GetDirectionVector(input);
            }
        }

        return Directions.None;
    }
}
