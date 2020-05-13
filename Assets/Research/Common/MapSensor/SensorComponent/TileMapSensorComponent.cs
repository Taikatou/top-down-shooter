﻿using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
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

        public BehaviorParameters behaviorParameters;

        public List<GridSpace> detectableTags;

        public List<GridSpace> selfDetectableTags;
        
        public List<GridSpace> adversaryDetectableTags;
        
        private TileMapSensor _tileMapSensor;
        
        private int ObservationStacks => mObservationStacks;

        protected GameObject LearningEnvironment { get; private set; }

        protected abstract TileMapSensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags);

        public override ISensor CreateSensor()
        {
            var newTags = new List<GridSpace>(detectableTags);
            newTags.AddRange(selfDetectableTags);
            newTags.AddRange(adversaryDetectableTags);
            LearningEnvironment = GetComponent<LearningEnvironmentAccessor>().learningEnvironment;
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
