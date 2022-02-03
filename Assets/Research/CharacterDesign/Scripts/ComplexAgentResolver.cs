using System;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Research.Common.Utils;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class ComplexAgentResolver : AgentResolver
    {
        private readonly SecondaryDirectionsInput _secondaryDirectionsInput;
        public AimControl aimControl = AimControl.EightWay;
        
        public override void OnActionReceivedImp(ActionBuffers vectorAction)
        {
            // Extrinsic Penalty
            var action = vectorAction.DiscreteActions[0];
            var primaryDirection = directionsKeyMapper.GetVectorDirection(action);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if (trainingSettings.shootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction.DiscreteActions[1]);
                inputManager.SetShootButton(shootButtonDown);
            }

            if (trainingSettings.secondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = AgentUtils.GetDecision(vectorAction.DiscreteActions[2], aimControl);
                var secondaryYInput = AgentUtils.GetDecision(vectorAction.DiscreteActions[3], aimControl);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                inputManager.SetAiSecondaryMovement(secondary);
            }

            if (trainingSettings.secondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction.DiscreteActions[4]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
        }
        
        public override void HeuristicImp(in ActionBuffers actions)
        {
            var actionsOut = actions.DiscreteActions;
            actionsOut[0] = (int) directionsKeyMapper.PrimaryDirections;
            if (trainingSettings.shootEnabled)
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                actionsOut[1] = Convert.ToInt32(shootButtonState);
            }

            if (trainingSettings.secondaryInputEnabled)
            {
                var secondaryDirections = _secondaryDirectionsInput.SecondaryDirection;
                actionsOut[2] = (int) secondaryDirections.x;
                actionsOut[3] = (int) secondaryDirections.y;
            }
            
            if (trainingSettings.secondaryAbilityEnabled)
            {
                var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                var secondaryShootButtonInput = Convert.ToInt32(secondaryShootButtonState);
                actionsOut[4] = secondaryShootButtonInput;
            }  
        }

        public override void OnEpisodeBegin()
        {
            
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(inputManager.PrimaryMovement);
            sensor.AddObservation(inputManager.SecondaryMovement);
        }

        public ComplexAgentResolver(TrainingSettings trainingSettings, DirectionsKeyMapper directionsKeyMapper, TopDownInputManager inputManager, SecondaryDirectionsInput secondaryDirectionsInput) : base(trainingSettings, directionsKeyMapper, inputManager)
        {
            _secondaryDirectionsInput = secondaryDirectionsInput;
        }
    }
}
