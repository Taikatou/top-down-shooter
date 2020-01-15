using MoreMountains.TopDownEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInputManager : InputManager
    {
        public bool aiEnabled = true;
        private MlAgentInput inputInterface;

        public void UpdateInterface()
        {
            if (inputInterface == null)
            {
                var inputs = FindObjectsOfType<MlAgentInput>();
                inputInterface = inputs[0];
            }
        }

        public override void SetMovement()
        {
            if (aiEnabled)
            {
                UpdateInterface();
                if (inputInterface != null)
                {
                    _primaryMovement = inputInterface.PrimaryInput;
                }
            }
            else
            {
                base.SetMovement();
            }
        }

        public override void SetSecondaryMovement()
        {
            if (aiEnabled)
            {
                UpdateInterface();
                if (inputInterface != null)
                {
                    _secondaryMovement = inputInterface.SecondaryInput;
                }
            }
            else
            {
                base.SetSecondaryMovement();
            }
        }
    }
}