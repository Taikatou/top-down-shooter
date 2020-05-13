﻿using System;
using System.Collections.Generic;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor
{
    public abstract class TileMapSensor : ISensor
    {
        protected int SizeX => _size - 1;
        protected int SizeY => _size - 1;

        private readonly int _size = 50;

        protected readonly GridSpace[,] MObservations;
        protected readonly int[] MShape;

        private readonly bool _debug;
        
        private readonly GameObject _learningEnvironment;

        private readonly int _teamId;
        
        protected readonly Dictionary<GridSpace, int> GridSpaceValues = new Dictionary<GridSpace, int>
            {
                { GridSpace.Floor, 0 },
                { GridSpace.Wall, 1 },
                { GridSpace.Self, 2 },
                { GridSpace.Team1, 3 },
                { GridSpace.Team2, 4 }
            };
        
        public abstract int WriteObservations(ObservationWriter writer);
        
        public void Reset() { Array.Clear(MObservations, 0, SizeX*SizeY); }
        
        public byte[] GetCompressedObservation() { return null; }


        private MapAccessor MapAccessor
        {
            get
            {
                if (_learningEnvironment != null)
                {
                    var mapAccessor = _learningEnvironment.GetComponentInChildren<MapAccessor>();
                    return mapAccessor;
                }
                return null;
            }
        }

        private GridSpace[,] GridSpaces
        {
            get
            {
                if (MapAccessor != null)
                {
                    return MapAccessor.Map;
                }
                return null;
            }
        }

        public TileMapSensor(GameObject learningEnvironment, int teamId, bool debug)
        {
            _learningEnvironment = learningEnvironment;
            _debug = debug;
            _teamId = teamId;

            var detectable = GridSpaceValues.Count;
            MShape = new[] { SizeX, SizeY, detectable };
            MObservations = new GridSpace[SizeX, SizeY];
        }

        public int[] GetObservationShape()
        {
            return MShape;
        }

        private static void OutputDebugMap(GridSpace [,] debugGrid)
        {
            var roomWidth = debugGrid.GetUpperBound(0);
            var roomHeight = debugGrid.GetUpperBound(1);
            var output = "Output log \n";
            for (var y = roomHeight; y >= 0; y--)
            {
                for (var x = 0; x < roomWidth; x++)
                {
                    output += (int) debugGrid[x, y];
                }
                output += "\n";
            }
            Debug.Log(output);
        }

        public int Write(ObservationWriter writer)
        {
            return WriteObservations(writer);
        }

        public void Update()
        {
            if (GridSpaces != null)
            {
                for (var y = 0; y < SizeY; y++)
                {
                    for (var x = 0; x < SizeX; x++)
                    {
                        MObservations[x, y] = GridSpaces[x, y];
                    }
                }

                var entities = _learningEnvironment.GetComponentsInChildren<EntityMapPosition>();
                
                foreach (var entity in entities)
                {
                    var position = entity.transform.position;
                    var cell = MapAccessor.GetPosition(position);
                    MObservations[cell.x, cell.y] = entity.GetType(_teamId);
                }
                
                if (_debug)
                {
                    OutputDebugMap(MObservations);
                }
                else
                {
                    Debug.Log("No debug trace");
                }
            }
            else
            {
                Debug.Log("Invalid grid array");
            }
        }

        public SensorCompressionType GetCompressionType()
        {
            return SensorCompressionType.None;
        }

        public string GetName()
        {
            return "TopDown Sensor";
        }
    }
}