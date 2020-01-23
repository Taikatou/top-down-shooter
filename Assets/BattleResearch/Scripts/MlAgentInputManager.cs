using System;
using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInputManager : InputManager
    {
        public bool aiEnabled = true;
        
        private MlAgentInput _inputInterface;
        
        public override MMInput.ButtonStates ReloadButtonState => _inputInterface.ReloadButtonState;

        public override MMInput.ButtonStates DashButtonState =>_inputInterface.DashButtonState;

        private bool CheckAi()
        {
            if (aiEnabled)
            {
                if (_inputInterface == null)
                {
                    var inputs = FindObjectsOfType<MlAgentInput>();
                    _inputInterface = inputs.SingleOrDefault(player => player.PlayerId == PlayerID);
                }
                return _inputInterface != null;
            }
            return false;
        }

        public override void SetMovement()
        {
            if (CheckAi())
            {
                _primaryMovement = _inputInterface.PrimaryInput;
            }
            else
            {
                base.SetMovement();
            }
        }

        public override void SetSecondaryMovement()
        {
            if (CheckAi())
            {
                _secondaryMovement = _inputInterface.SecondaryInput;
            }
            else
            {
                base.SetSecondaryMovement();
            }
        }
        
        protected override void SetShootAxis()
        {
            if (CheckAi())
            {
                ShootAxis = _inputInterface.ShootButtonState;
            }
            else
            {
                base.SetShootAxis();
            }
        }
    }
}