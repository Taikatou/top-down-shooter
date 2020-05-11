using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class TileMapSensorComponent : SensorComponent
    {
        public bool debug;
        
        [Range(1, 50)]
        [Tooltip("Number of raycast results that will be stacked before being fed to the neural network.")]
        public int mObservationStacks = 1;

        private int ObservationStacks => mObservationStacks;

        private TileMapSensor _tileMapSensor;

        public GameObject learningEnvironment;
        
        public BehaviorParameters behaviorParameters;

        public override ISensor CreateSensor()
        {
            _tileMapSensor = new TileMapSensor(learningEnvironment, behaviorParameters.TeamId, debug);
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
            return new [] {shape[0], shape[1], shape[2], ObservationStacks};
        }
    }
}
