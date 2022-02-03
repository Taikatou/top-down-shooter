using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.CharacterDesign.Scripts.Environment;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Actuators
{
    public enum EGunActions { None, RotateClockwise, RotateAntiClockwise, ShootGun, SecondaryAbility}
    
    public abstract class TopDownActuator : IActuator
    {
        private readonly MlCharacter _character;
        protected abstract void ImpOnActionReceived(ActionBuffers actionBuffers);

        public abstract void Heuristic(in ActionBuffers actionBuffersOut);

        public abstract ActionSpec ActionSpec { get; }
        public abstract string Name { get; }

        public virtual void ResetData() { }

        public virtual void WriteDiscreteActionMask(IDiscreteActionMask actionMask) { }

        protected TopDownActuator(MlCharacter character)
        {
            _character = character;
        }
        
        public void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
            {
                ImpOnActionReceived(actionBuffers);
            }
        }
    }
    public class GunActuator : TopDownActuator
    {
        [Observable]
        private float Rotation { get; set; }

        private readonly float _turnRate = 3f;
        private readonly TopDownInputManager _inputManager;
        private readonly int _actionIndex;
        public override ActionSpec ActionSpec { get; }
        public override string Name => "GunActuator";
        
        public static ActionSpec CompActionSpec => ActionSpec.MakeDiscrete(Enum.GetNames(typeof(EGunActions)).Length);

        public GunActuator(TopDownInputManager inputManager, int actionIndex, ActionSpec actionSpec, MlCharacter character) : base(character)
        {
            _inputManager = inputManager;
            _actionIndex = actionIndex;
            ActionSpec = actionSpec;
        }

        protected override void ImpOnActionReceived(ActionBuffers actionBuffers)
        {
            var action = (EGunActions) actionBuffers.DiscreteActions[_actionIndex];

            _inputManager.SetShootButton(action == EGunActions.ShootGun);
            _inputManager.SetSecondaryShootButton(action == EGunActions.SecondaryAbility);
            switch (action)
            {
                case EGunActions.RotateClockwise:
                    Rotation += _turnRate;
                    break;
                case EGunActions.RotateAntiClockwise:
                    Rotation -= _turnRate;
                    break;
            }
            Rotation %= 360.0f;
        
            var radians =  Rotation * Mathf.Deg2Rad;
            var angle = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            _inputManager.SetAiSecondaryMovement(angle);
        }

        public override void Heuristic(in ActionBuffers actionBuffersOut)
        {
            var action = EGunActions.None;
            var discreteActions = actionBuffersOut.DiscreteActions;
            
            var turnRight = Input.GetKey(KeyCode.Home);
            var turnLeft = Input.GetKey(KeyCode.End);
            if (turnRight ^ turnLeft)
            {
                action = turnRight ? EGunActions.RotateClockwise : EGunActions.RotateAntiClockwise;
            }
            else
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                var secondaryButtonState = Input.GetKey(KeyCode.C);
                if (shootButtonState)
                {
                    action = EGunActions.ShootGun;    
                }
                else if (secondaryButtonState)
                {
                    action = EGunActions.SecondaryAbility;  
                }
            }
            discreteActions[_actionIndex] = (int) action;
        }
        public override void ResetData()
        {
            Rotation = 0;
            _inputManager.SetShootButton(false);
        }
    }
}
