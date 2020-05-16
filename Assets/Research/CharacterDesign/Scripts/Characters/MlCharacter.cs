using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using Unity.MLAgents;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlCharacter : Character
    {
        public override void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
        {
            base.RespawnAt(spawnPoint, facingDirection);
        }

        public override void SetInputManager()
        {
            LinkedInputManager = GetComponent<TopDownInputManager>();
            UpdateInputManagersInAbilities();
        }
    }
}
