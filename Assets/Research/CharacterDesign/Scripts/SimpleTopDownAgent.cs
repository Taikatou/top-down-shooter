using Research.Common;
using Unity.MLAgents.Sensors;
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
        
        protected virtual bool ShootButtonState => Input.GetKey(KeyCode.X);
        protected override void OnActionReceivedImp(float[] vectorAction)
        {
            var action = (Controls) vectorAction[0];
            inputManager.SetShootButton(action == Controls.Shoot);
            
            var isMovement = IsMovement(action);
            var movementAction = isMovement ? action : Controls.None;
            var primaryDirection = directionsKeyMapper.GetVectorDirection((Directions)movementAction);
            
            if(!isMovement)
            {
                switch (action)
                {
                    case Controls.Shoot:
                        break;
                    case Controls.AimClock:
                        _rotation += turnRate;
                        break;
                    case Controls.AimAntiClock:
                        _rotation -= turnRate;
                        break;
                }
                _rotation %= 360.0f; 
            }
            
            inputManager.SetAiPrimaryMovement(primaryDirection);

            var radians =  _rotation * Mathf.Deg2Rad;
            var angle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            inputManager.SetAiSecondaryMovement(angle);
        }

        protected override void HeuristicImp(float[] actionsOut)
        {
            var move = directionsKeyMapper.PrimaryDirections;

            if (IsMovement((Controls)move))
            {
                actionsOut[0] = (int) move;
            }
            else
            {
                var turnRight = Input.GetKey(KeyCode.Home);
                var turnLeft = Input.GetKey(KeyCode.End);
                if (turnRight ^ turnLeft)
                {
                    var action = turnRight ? Controls.AimClock : Controls.AimAntiClock;
                    actionsOut[0] = (int) action;
                }
                else
                {
                    var action = ShootButtonState? Controls.Shoot : Controls.None;
                    actionsOut[0] = (int) action;
                }
            }
        }
        
        private static bool IsMovement(Controls control)	
        {	
            return control == Controls.Down || control == Controls.Left || 
                   control == Controls.Right || control == Controls.Up;	
        }

        protected override void ObserveWeapon(VectorSensor sensor)
        {
            sensor.AddObservation(_rotation / 360.0f);
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            _rotation = 0;
            inputManager.SetShootButton(false);
        }
    }
}
