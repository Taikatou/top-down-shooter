using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public abstract class TileMapSensorComponent : Unity.MLAgents.Sensors.SensorComponent
    {
        public bool debug;
        
        [Range(1, 50)]
        [Tooltip("Number of raycast results that will be stacked before being fed to the neural network.")]
        public int mObservationStacks = 1;

        private int ObservationStacks => mObservationStacks;

        private TileMapSensor _tileMapSensor;

        public GameObject learningEnvironment;
        
        public BehaviorParameters behaviorParameters;
        
        
        public List<GridSpace> detectableTags;
        
        public List<GridSpace> selfDetectableTags;
        
        public List<GridSpace> adversaryDetectableTags;

        protected abstract TileMapSensor CreateTileMapSensor(List<GridSpace> detectTags);

        public override ISensor CreateSensor()
        {
            var newTags = new List<GridSpace>(detectableTags);
            newTags.AddRange(selfDetectableTags);
            newTags.AddRange(adversaryDetectableTags);
            _tileMapSensor = CreateTileMapSensor(newTags);
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
