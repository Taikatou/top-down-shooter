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
    }
}
