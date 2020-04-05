using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research
{
    public class GunOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<ProjectileWeapon>()?.Owner; return owner != null && owner.GetComponentInChildren<SpriteOutline>().IsBlue;
            }
        }

        private void Start()
        {
            outline.IsBlue = ParentBlue;
        }
    }
}
