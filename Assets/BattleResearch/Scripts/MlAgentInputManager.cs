using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInputManager : InputManager
    {
        public bool aiEnabled = true;

        public bool Enabled => aiEnabled && InputInterface != null;
        
        private MlAgentInput _inputInterface;

        private MlAgentInput InputInterface
        {
            get
            {
                if (_inputInterface == null)
                {
                    var inputs = FindObjectsOfType<MlAgentInput>();
                    _inputInterface = Array.Find(inputs, player => player.PlayerId == PlayerID);
                }
                return _inputInterface;
            }
        }

        public override MMInput.ButtonStates ReloadButtonState => Enabled ? InputInterface.ReloadButtonState : base.ReloadButtonState;

        public override MMInput.ButtonStates ShootButtonState => Enabled ? InputInterface.ShootButtonState : base.ShootButtonState;
		
        public override MMInput.ButtonStates SecondaryShootButtonState => Enabled ? InputInterface.SecondaryButtonState : base.SecondaryShootButtonState;

        public override void SetMovement()
        {
            if (Enabled)
            {
                _primaryMovement = InputInterface.PrimaryInput;
            }
            else
            {
                base.SetMovement();
            }
        }

        public override void SetSecondaryMovement()
        {
            if (Enabled)
            {
                _secondaryMovement = InputInterface.SecondaryInput;
            }
            else
            {
                base.SetSecondaryMovement();
            }
        }
    }
}