using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        public bool normalize;

        protected override TileMapSensor CreateTileMapSensor(List<GridSpace> detectTags)
        {
            return new TileMapSensor2D(learningEnvironment, behaviorParameters.TeamId, debug, normalize, detectTags);
        }
    }
}
