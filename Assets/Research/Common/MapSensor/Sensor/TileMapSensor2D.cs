﻿using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor
{
    public class TileMapSensor2D : TileMapSensor
    {
        private readonly bool _normalize;

        public override int WriteObservations(ObservationWriter writer)
        {
            for (var y = 0; y < SizeY; y++)
            {
                for (var x = 0; x < SizeX; x++)
                {
                    var space = (float) MObservations[x, y];
                    writer[x, y, 0] =  _normalize? space/GridSpaceValues.Count : space;
                }
            }
            var outputSize = MShape[0] * MShape[1] * MShape[2];
            return outputSize;
        }

        public TileMapSensor2D(GameObject learningEnvironment, int teamId, bool debug, bool normalize) : 
            base(learningEnvironment, teamId, debug)
        {
            _normalize = normalize;
        }
    }
}