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

        public List<GridSpace> detectableTags;
        
        protected ISensor TileMapSensor;
        
        protected int GetTeamId => GetComponent<BehaviorParameters>().TeamId;

        protected MapAccessor MapAccessor => GetComponentInParent<MapSensorGetter>().mapAccessor;

        protected GetEnvironmentMapPositions EnvironmentInstance => GetComponentInParent<GetEnvironmentMapPositions>();

        protected abstract ISensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags);

        public override ISensor CreateSensor()
        {
            TileMapSensor = CreateTileMapSensor(detectableTags);
            return TileMapSensor;
        }
    }
}
