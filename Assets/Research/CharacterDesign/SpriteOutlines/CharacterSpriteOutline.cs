using Research.Common.MapSensor.GridSpaceEntity;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    [ExecuteInEditMode]
    public class CharacterSpriteOutline : SpriteOutline
    {
        public BehaviourGetTeamId teamId;

        // Update is called once per frame
        public override bool IsPrior =>  teamId.GetTeamId == 1;
    }
}
