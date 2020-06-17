using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;

namespace Research.Common.MapSensor.Sensor.SensorData
{
    public abstract class BaseSensorData
    {
        protected TileMapSensorConfig Config;

        protected BaseSensorData(TileMapSensorConfig config)
        {
            Config = config;
        }

        public abstract void UpdateMap(GridSpace[,] map);

        public abstract void UpdateMapEntityPositions(GridSpace[,] observations, BaseMapPosition[] entityMapPositions);
    }
}
