using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInput : MonoBehaviour
    {
        public Vector2 PrimaryInput { get; set; }

        public Vector2 SecondaryInput { get; set; }

        public MMInput.ButtonStates ShootButtonState { get; private set; }
        
        public MMInput.ButtonStates SecondaryButtonState { get; private set; }
        
        public MMInput.ButtonStates ReloadButtonState { get; private set; }
        
        public string PlayerId
        {
            get
            {
                var character = GetComponent<Character>();
                return character.PlayerID;
            }
        }

        private void Start()
        {
            ShootButtonState = MMInput.ButtonStates.ButtonUp;
        }

        public void SetShootButtonState(bool active)
        {
            ShootButtonState = SetButton(ShootButtonState, active);
        }
        
        public void SetSecondaryShootButtonState(bool active)
        {
            SecondaryButtonState = SetButton(SecondaryButtonState, active);
        }

        public void SetReloadButtonState(bool active)
        {
            ReloadButtonState = SetButton(ReloadButtonState, active);
        }
        

        private MMInput.ButtonStates SetButton(MMInput.ButtonStates buttonState, bool active)
        {
            if (active)
            {
                switch (buttonState)
                {
                    case MMInput.ButtonStates.ButtonDown:
                        buttonState = MMInput.ButtonStates.ButtonPressed;
                        break;

                    case MMInput.ButtonStates.ButtonUp:
                        buttonState = MMInput.ButtonStates.ButtonDown;
                        break;
                    
                    case MMInput.ButtonStates.Off:
                        buttonState = MMInput.ButtonStates.ButtonPressed;
                        break;
                }
            }
            else
            {
                switch (buttonState)
                {
                    case MMInput.ButtonStates.ButtonDown:
                        buttonState = MMInput.ButtonStates.ButtonUp;
                        break;
                    
                    case MMInput.ButtonStates.ButtonPressed:
                        buttonState = MMInput.ButtonStates.ButtonUp;
                        break;
                    
                    case MMInput.ButtonStates.Off:
                        buttonState = MMInput.ButtonStates.ButtonUp;
                        break;
                }
            }

            return buttonState;
        }
    }
}
