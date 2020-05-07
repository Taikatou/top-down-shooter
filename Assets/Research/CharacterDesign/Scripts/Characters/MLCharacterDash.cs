using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace Characters
{
    public class MLCharacterDash : CharacterDash2D
    {
        protected override void HandleInput()
        {
            base.HandleInput();
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }
            if (_inputManager.SecondaryShootButtonState == MMInput.ButtonStates.ButtonDown)
            {
                DashStart();
            }
        }
    }
}
