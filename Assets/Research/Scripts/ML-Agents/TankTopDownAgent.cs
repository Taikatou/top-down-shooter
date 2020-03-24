using System;
using UnityEngine;

namespace Research.Scripts
{
    public class TankTopDownAgent : TopDownAgent
    {
        public override void OnActionReceived(float[] vectorAction)
        {
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[0]);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            // Shoot Button Input
            var shootButtonDown = Convert.ToBoolean(vectorAction[1]);
            inputManager.SetShootButton(shootButtonDown);

            if (vectorAction.Length >= 5)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[2]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
        }
        
        public override float[] Heuristic()
        {
            var shootButtonState = Input.GetKey(KeyCode.X);
            var shootButtonInput = Convert.ToSingle(shootButtonState);
            
            var secondaryShootButtonState = Input.GetKey(KeyCode.C);
            var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);

            var output = new []
            {
                (float)directionsKeyMapper.PrimaryDirections,
                shootButtonInput,
                secondaryShootButtonInput
            };
            
            return output;
        }
    }
}
