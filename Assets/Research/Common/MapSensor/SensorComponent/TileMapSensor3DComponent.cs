using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        protected override TileMapSensor CreateTileMapSensor(List<GridSpace> detectTags)
        {
            return new TileMapSensor3D(learningEnvironment, behaviorParameters.TeamId, debug, detectTags);
        }
    }
}
