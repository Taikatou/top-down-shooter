using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    [ExecuteInEditMode]
    public class CharacterSpriteOutline : SpriteOutline
    {
        public BehaviorParameters teamId;

        // Update is called once per frame
        public override bool IsPrior =>  teamId.TeamId == 1;
    }
}
