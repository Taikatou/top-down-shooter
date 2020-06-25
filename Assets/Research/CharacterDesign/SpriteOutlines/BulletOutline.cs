using Research.CharacterDesign.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    public class BulletOutline : SpriteOutline
    {
        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<TeamDamageOnTouch>()?.Owner;
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().IsPrior;
            }
        }

        public override bool IsPrior => ParentBlue;
    }
}
