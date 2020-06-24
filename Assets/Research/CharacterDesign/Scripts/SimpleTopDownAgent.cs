using System;
using Research.Common;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public enum GunControls {
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
            var gunAction = (GunControls) vectorAction[1];
            inputManager.SetShootButton(gunAction == GunControls.Shoot);
            switch (gunAction)
            {
                case GunControls.Shoot:
                    Debug.Log("Shoot");
                    break;
                case GunControls.AimClock:
                    _rotation += turnRate;
                    break;
                case GunControls.AimAntiClock:
                    _rotation -= turnRate;
                    break;
            }
            _rotation %= 360.0f;

            var movementAction = (Directions) (vectorAction[0]);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(movementAction);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            var radians =  _rotation * Mathf.Deg2Rad;
            var angle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            inputManager.SetAiSecondaryMovement(angle);
        }

        protected override void HeuristicImp(float[] actionsOut)
        {
            actionsOut[0] = (float) directionsKeyMapper.PrimaryDirections;

            var turnRight = Input.GetKey(KeyCode.Home);
            var turnLeft = Input.GetKey(KeyCode.End);
            if (turnRight ^ turnLeft)
            {
                var action = turnRight ? GunControls.AimClock : GunControls.AimAntiClock;
                actionsOut[1] = (int) action;
            }
            else
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                actionsOut[1] = shootButtonState ? (int)GunControls.Shoot : -1;
            }
        }

        protected override void ObserveWeapon(VectorSensor sensor)
        {
            sensor.AddObservation(_rotation / 360.0f);
        }
    }
}
