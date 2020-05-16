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
        [Range(1, 50)]
        [Tooltip("Number of raycast results that will be stacked before being fed to the neural network.")]
        public int mObservationStacks = 1;
        
        public bool debug;
        
        public int tileMapSize;

        public bool trackPosition;

        public string sensorName;

        public List<GridSpace> detectableTags;

        public int TeamId => GetComponent<BehaviorParameters>().TeamId;

        private TileMapSensor _tileMapSensor;

        private int ObservationStacks => mObservationStacks;

        protected MapAccessor MapAccessor => GetComponent<MapSensorGetter>().mapAccessor;

        protected EnvironmentInstance EnvironmentInstance => GetComponent<MapSensorGetter>().environmentInstance;

        protected abstract TileMapSensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags);

        public override ISensor CreateSensor()
        {
            _tileMapSensor = CreateTileMapSensor(detectableTags);
            if (ObservationStacks != 1)
            {
                var stackingSensor = new StackingSensor(_tileMapSensor, ObservationStacks);
                return stackingSensor;
            }
            return _tileMapSensor;
        }

        public override int[] GetObservationShape()
        {
            var stacks = ObservationStacks > 1 ? ObservationStacks : 1;
            var shape = _tileMapSensor.GetObservationShape();
            return new [] { shape[0], shape[1], shape[2], stacks };
        }
    }
}
