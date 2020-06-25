using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    public class GunOutline : SpriteOutline
    {
        private bool ParentPrior
        {
            get
            {
                var owner = GetComponentInParent<ProjectileWeapon>()?.Owner; 
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().IsPrior;
            }
        }

        public override bool IsPrior => ParentPrior;
    }
}
