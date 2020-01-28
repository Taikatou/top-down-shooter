using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInput : MonoBehaviour, ISense
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

        public Dictionary<string, float> GetObservations()
        {
            var obs = new Dictionary<string, float>()
            {
                { "Shoot Button State", (float)ShootButtonState },
                { "SecondaryButtonState", (float)SecondaryButtonState },
                { "ReloadButtonState", (float) ReloadButtonState }
            };
            return obs;
        }

        public string SenseName => "Agent Sense";
    }
}
