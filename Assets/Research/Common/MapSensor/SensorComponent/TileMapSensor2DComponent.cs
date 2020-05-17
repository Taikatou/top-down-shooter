using System.Collections.Generic;
using Research.Common.MapSensor.Sensor;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.MapSensor.SensorComponent
{
    public class TileMapSensor2DComponent : TileMapSensorComponent
    {
        [Range(1, 50)]
        [Tooltip("Number of raycast results that will be stacked before being fed to the neural network.")]
        public int mObservationStacks = 1;

        private int _outputSizeLinear;
        protected override ISensor CreateTileMapSensor(IEnumerable<GridSpace> detectTags)
        {
            var twoDSensor = new TileMapSensor2D(sensorName,
                                                tileMapSize,
                                                trackPosition, 
                                                debug, 
                                                detectTags,
                                                MapAccessor,
                                                EnvironmentInstance,
                                                GetTeamId,
                                                buffer);
            _outputSizeLinear = twoDSensor.Config.OutputSizeLinear;
            ISensor returnSensor;
            if (mObservationStacks != 1)
            {
                var stackingSensor = new StackingSensor(twoDSensor, mObservationStacks);
                returnSensor = stackingSensor;
            }
            else
            {
                returnSensor = twoDSensor;
            }

            return returnSensor;
        }
        
        public override int[] GetObservationShape()
        {
            var stacks = mObservationStacks > 1 ? mObservationStacks : 1;
            return new [] { _outputSizeLinear * stacks };
        }
    }
}
