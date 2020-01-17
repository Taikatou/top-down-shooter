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

        public MMInput.ButtonStates ShootButtonState { get; set; }

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

        public void SetButton(bool active)
        {
            if (active)
            {
                switch (ShootButtonState)
                {
                    case MMInput.ButtonStates.ButtonDown:
                        ShootButtonState = MMInput.ButtonStates.ButtonPressed;
                        break;

                    case MMInput.ButtonStates.ButtonUp:
                        ShootButtonState = MMInput.ButtonStates.ButtonDown;
                        break;
                }
            }
            else
            {
                switch (ShootButtonState)
                {
                    case MMInput.ButtonStates.ButtonDown:
                        ShootButtonState = MMInput.ButtonStates.ButtonUp;
                        break;
                    
                    case MMInput.ButtonStates.ButtonPressed:
                        ShootButtonState = MMInput.ButtonStates.ButtonUp;
                        break;
                }
            }
        }
    }
}
