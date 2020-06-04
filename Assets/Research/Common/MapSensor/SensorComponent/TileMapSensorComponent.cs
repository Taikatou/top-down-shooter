using System.Collections.Generic;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

namespace Research.Common.MapSensor.SensorComponent
{
    public abstract class TileMapSensorComponent : Unity.MLAgents.Sensors.SensorComponent
    {
        public bool debug;
        
        public int tileMapSize;

        public bool trackPosition;

        public string sensorName;

        public bool buffer;
        
        public MapAccessor mapAccessor;

        public List<GridSpace> detectableTags;
        
        protected ISensor TileMapSensor;
        
        protected int GetTeamId => GetComponent<BehaviorParameters>().TeamId;

        protected GetEnvironmentMapPositions EnvironmentInstance => GetComponentInParent<GetEnvironmentMapPositions>();
    }
}
