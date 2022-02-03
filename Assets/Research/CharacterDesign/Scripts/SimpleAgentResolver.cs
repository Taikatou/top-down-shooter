using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public enum Controls {
        None = EDirections.None, 
        Left = EDirections.Left, 
        Right = EDirections.Right, 
        Up = EDirections.Up,
        Down = EDirections.Down,
        Shoot,
        AimClock,
        AimAntiClock,
        StopRotate
    }
    public sealed class SimpleAgentResolver : AgentResolver
    {
        private float _rotation;
        public float turnRate = 3f;

        [Observable]
        private Controls _shootControl;
        
        [Observable]
        private Controls _moveControl;

        private bool ShootButtonState => Input.GetKey(KeyCode.X);
        
        public SimpleAgentResolver(TrainingSettings trainingSettings, DirectionsKeyMapper directionsKeyMapper, TopDownInputManager inputManager) : base(trainingSettings, directionsKeyMapper, inputManager)
        {
        }
       
        public override void OnActionReceivedImp(ActionBuffers vectorAction)
        {
            var action = (Controls) vectorAction.DiscreteActions[0];

            if (action == Controls.None)
            {
                _moveControl = Controls.None;
                _shootControl = Controls.None;
            }
            else
            {
                var isMovement = IsMovement(action);
                if (isMovement)
                {
                    _moveControl = action;
                }
                else
                {
                    _shootControl = action;
                    Debug.Log(action);
                }
            }

            MoveAction();
            GunAction();

            var radians =  _rotation * Mathf.Deg2Rad;
            var angle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            inputManager.SetAiSecondaryMovement(angle);
        }

        private void MoveAction()
        {
            var primaryDirection = directionsKeyMapper.GetVectorDirection((EDirections)_moveControl);
            inputManager.SetAiPrimaryMovement(primaryDirection);
        }

        private void GunAction()
        {
            inputManager.SetShootButton(_shootControl == Controls.Shoot);
            switch (_shootControl)
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

        public override void HeuristicImp(in ActionBuffers actions)
        {
            var move = directionsKeyMapper.PrimaryDirections;

            var actionsOut = actions.DiscreteActions;
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

        public override void OnEpisodeBegin()
        {
            _rotation = 0;
            inputManager.SetShootButton(false);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(_rotation / 360f);
        }
    }
}
