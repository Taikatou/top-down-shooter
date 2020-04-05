using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts.Characters
{
    public class MlCharacter : Character
    {
        public override void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
        {
            base.RespawnAt(spawnPoint, facingDirection);
            
            var requester = GetComponent<DecisionRequester>();
            
            requester.allowDecisions = true;
        }
    }
}
