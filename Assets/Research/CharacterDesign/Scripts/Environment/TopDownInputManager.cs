using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class TopDownInputManager : InputManager
    {
        private MMInput.ButtonStates _shootButtonState;

        private MMInput.ButtonStates _secondaryButtonState;

        private MMInput.ButtonStates _reloadButtonState;
        
        public override Vector2 PrimaryMovement => _aiPrimaryMovement;

        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public override Vector2 SecondaryMovement => _aiSecondaryMovement;

        public override MMInput.ButtonStates ReloadButtonState => _reloadButtonState;

        public override MMInput.ButtonStates ShootButtonState => _shootButtonState;

        public override MMInput.ButtonStates SecondaryShootButtonState => _secondaryButtonState;

        private Vector2 _aiPrimaryMovement;

        private Vector2 _aiSecondaryMovement;

        public void SetAiPrimaryMovement(Vector2 vector)
        {
            _aiPrimaryMovement.x = vector.x;
            _aiPrimaryMovement.y = vector.y;
        }

        public void SetAiSecondaryMovement(Vector2 vector)
        {
            _aiSecondaryMovement.x = vector.x;
            _aiSecondaryMovement.y = vector.y;
        }

        public void MoveAiSecondaryMovement(Vector2 vector, float scale = 0.01f)
        {
            var change = vector * scale;
            _aiSecondaryMovement.x = Mathf.Clamp(_aiSecondaryMovement.x + change.x, -1.0f, 1.0f);
            _aiSecondaryMovement.y = Mathf.Clamp(_aiSecondaryMovement.y + change.y, -1.0f, 1.0f);
        }

        public override void SetMovement()
        {
            
        }
        
        public void SetReloadButton(bool active)
        {
            _reloadButtonState = SetButton(_reloadButtonState, active);
        }
        
        public void SetShootButton(bool active)
        {
            _shootButtonState = SetButton(_shootButtonState, active);
        }
        
        public void SetSecondaryShootButton(bool active)
        {
            _secondaryButtonState = SetButton(_secondaryButtonState, active);
        }
        
        private MMInput.ButtonStates SetButton(MMInput.ButtonStates buttonState, bool down)
        {
            switch (buttonState)
            {
                case MMInput.ButtonStates.ButtonDown:
                    buttonState = down? MMInput.ButtonStates.ButtonPressed: MMInput.ButtonStates.ButtonUp;
                    break;

                case MMInput.ButtonStates.ButtonUp:
                    buttonState = down? MMInput.ButtonStates.ButtonDown: MMInput.ButtonStates.Off;
                    break;
                
                case MMInput.ButtonStates.Off:
                    buttonState = down? MMInput.ButtonStates.ButtonPressed : MMInput.ButtonStates.Off;
                    break;

                case MMInput.ButtonStates.ButtonPressed:
                    buttonState = down? MMInput.ButtonStates.ButtonDown : MMInput.ButtonStates.ButtonUp;
                    break;
            }

            return buttonState;
        }
    }
}
