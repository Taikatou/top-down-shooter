using Research.Common.MapSensor.GridSpaceEntity;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public abstract class TileMapSensorComponent : Unity.MLAgents.Sensors.SensorComponent
    {
        public string sensorName;

        public MapAccessor mapAccessor;

        protected ISensor TileMapSensor;
        
        public TileMapSensorConfig sileMapSensorConfig;
        
        protected int GetTeamId => GetComponent<BehaviorParameters>().TeamId;

        protected GetEnvironmentMapPositions EnvironmentInstance => GetComponentInParent<GetEnvironmentMapPositions>();
    }
}
