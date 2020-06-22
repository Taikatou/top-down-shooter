using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    public class GunOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<ProjectileWeapon>()?.Owner; 
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().isPrior;
            }
        }

        private void Start()
        {
            outline.isPrior = ParentBlue;
        }
    }
}
