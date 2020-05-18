using System.Collections.Generic;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

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

        protected int GetTeamId => GetComponent<BehaviorParameters>().TeamId;

        protected ISensor TileMapSensor;

        protected MapAccessor MapAccessor => GetComponentInParent<MapSensorGetter>().mapAccessor;

        protected EnvironmentInstance EnvironmentInstance => GetComponentInParent<MapSensorGetter>().environmentInstance;

        protected abstract ISensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags);

        public override ISensor CreateSensor()
        {
            TileMapSensor = CreateTileMapSensor(detectableTags);
            return TileMapSensor;
        }
    }
}
