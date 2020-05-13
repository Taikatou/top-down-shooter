using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        public bool normalize;

        protected override TileMapSensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags)
        {
            return new TileMapSensor2D( LearningEnvironment, 
                                        sensorName,
                                        tileMapSize,
                                        trackPosition, 
                                        debug, 
                                        detectTags, 
                                        normalize);
        }
    }
}
