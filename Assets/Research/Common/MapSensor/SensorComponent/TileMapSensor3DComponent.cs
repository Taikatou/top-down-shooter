using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor3DComponent : TileMapSensorComponent
    {
        protected override TileMapSensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags)
        {
            return new TileMapSensor3D( sensorName, 
                                        tileMapSize, 
                                        trackPosition, 
                                        debug, 
                                        detectTags, 
                                        mapAccessor, 
                                        environmentInstance);
        }
    }
}
