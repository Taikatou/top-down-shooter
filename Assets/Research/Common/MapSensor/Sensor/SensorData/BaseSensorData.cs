using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.Scripts.MLAgents;
using UnityEngine;

namespace Research.Common.MapSensor.Sensor.SensorData
{
    public abstract class BaseSensorData : MonoBehaviour
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
