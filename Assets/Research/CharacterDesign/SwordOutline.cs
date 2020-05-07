using Characters;
using UnityEngine;

namespace Research
{
    public class SwordOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<TopDownMeleeWeapon>()?.Owner;
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().IsBlue;
            }
        }

        private void Start()
        {
            outline.IsBlue = ParentBlue;
        }
    }
}
