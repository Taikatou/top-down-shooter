using MoreMountains.TopDownEngine;

namespace BattleResearch.Scripts
{
    public class MlAgentInputManager : InputManager
    {
        public bool aiEnabled = true;
        public IInput inputInterface;
        
        public override void SetMovement()
        {
            if (aiEnabled)
            {
                _primaryMovement.x = inputInterface.XAxis;
                _primaryMovement.y = inputInterface.YAxis;
            }
            else
            {
                base.SetMovement();
            }
        }
    }
}