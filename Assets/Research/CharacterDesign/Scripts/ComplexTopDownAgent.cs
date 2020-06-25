using System;
using Research.Common;
using Research.Common.Utils;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class ComplexTopDownAgent : TopDownAgent
    {
            
        public AimControl aimControl = AimControl.EightWay;
        
        protected override void OnActionReceivedImp(float[] vectorAction)
        {
            var counter = 0;
            // Extrinsic Penalty
            var action = Mathf.FloorToInt(vectorAction[counter++]);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(action);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if (trainingSettings.shootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetShootButton(shootButtonDown);
            }

            if (trainingSettings.secondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = AgentUtils.GetDecision(vectorAction[counter++], aimControl);
                var secondaryYInput = AgentUtils.GetDecision(vectorAction[counter++], aimControl);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                inputManager.SetAiSecondaryMovement(secondary);
            }

            if (trainingSettings.secondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[counter]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
        }
        
        protected override void HeuristicImp(float[] actionsOut)
        {
            var index = 0;
            actionsOut[index++] = (int) directionsKeyMapper.PrimaryDirections;
            if (trainingSettings.shootEnabled)
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                actionsOut[index++] = Convert.ToSingle(shootButtonState);
            }

            if (trainingSettings.secondaryInputEnabled)
            {
                var secondaryDirections = secondaryDirectionsInput.SecondaryDirection;
                actionsOut[index++] = secondaryDirections.x;
                actionsOut[index++] = secondaryDirections.y;
            }
            
            if (trainingSettings.secondaryAbilityEnabled)
            {
                var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);
                actionsOut[index] = secondaryShootButtonInput;
            }  
        }
        
        protected override void ObserveWeapon(VectorSensor sensor)
        {
            sensor.AddObservation(inputManager.PrimaryMovement);
            sensor.AddObservation(inputManager.SecondaryMovement);
        }
    }
}
