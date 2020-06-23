using System;
using Research.Common;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public enum Controls { 
        None = Directions.None,
        Left = Directions.Left, 
        Right = Directions.Right, 
        Up = Directions.Up, 
        Down = Directions.Down,
        Shoot,
        AimClock,
        AimAntiClock
    }
    public class SimpleTopDownAgent : TopDownAgent
    {
        private float _rotation;
        public float turnRate = 0.1f;
        protected override void OnActionReceivedImp(float[] vectorAction)
        {
            var action = (Controls)vectorAction[0];
            var setShoot = action == Controls.Shoot;
            inputManager.SetShootButton(setShoot);
            
            var isMovement = IsMovement(action);
            var movementAction = isMovement ? action : Controls.None;
            
            var primaryDirection = directionsKeyMapper.GetVectorDirection((int)movementAction);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if(!isMovement)
            {
                switch (action)
                {
                    case Controls.AimClock:
                        _rotation += turnRate;
                        break;
                    case Controls.AimAntiClock:
                        _rotation -= turnRate;
                        break;
                }
            }

            var radians =  _rotation * Mathf.Deg2Rad;
            var angle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            inputManager.SetAiSecondaryMovement(angle);
        }

        private static bool IsMovement(Controls control)
        {
            return control == Controls.Down || control == Controls.Left || control == Controls.Right ||
                   control == Controls.Up;
        }

        protected override void HeuristicImp(float[] actionsOut)
        {
            var move = directionsKeyMapper.PrimaryDirections;
            
            if (IsMovement((Controls) move))
            {
                actionsOut[0] = (int) move;
            }
            else
            {
                actionsOut[0] = (int) Controls.None;
                
                var turnRight = Input.GetKey(KeyCode.Home);
                var turnLeft = Input.GetKey(KeyCode.End);
                if (turnRight ^ turnLeft)
                {
                    var action = turnRight ? Controls.AimClock : Controls.AimAntiClock;
                    actionsOut[0] = (int) action;
                }
                
                var shootButtonState = Input.GetKey(KeyCode.X);
                if (shootButtonState)
                {
                    actionsOut[0] = (int)Controls.Shoot;
                }
            }
        }
    }
}
