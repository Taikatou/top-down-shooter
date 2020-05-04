using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlCharacter : Character
    {
        public override void RespawnAt(Transform spawnPoint, FacingDirections facingDirection)
        {
            base.RespawnAt(spawnPoint, facingDirection);
            
            var requester = GetComponent<DecisionRequester>();

            if (requester)
            {
                Academy.Instance.AgentPreStep += requester.MakeRequests;   
            }
        }
    }
}
