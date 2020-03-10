using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts
{
    public class TopDownInputManager : InputManager
    {
        public override Vector2 PrimaryMovement => _aiPrimaryMovement;

        /// the secondary movement (usually the right stick on a gamepad), used to aim
        public override Vector2 SecondaryMovement => _aiSecondaryMovement;

        private Vector2 _aiPrimaryMovement;

        private Vector2 _aiSecondaryMovement;

        public void SetAIPrimaryMovement(float x, float y)
        {
            _aiPrimaryMovement.x = x;
            _aiPrimaryMovement.y = y;
        }

        public void SetAISecondaryMovement(float x, float y)
        {
            _aiSecondaryMovement.x = x;
            _aiSecondaryMovement.y = y;
        }
    }
}
