using Characters;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    public class SwordOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<TopDownMeleeWeapon>()?.Owner;
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().isPrior;
            }
        }

        private void Start()
        {
            outline.isPrior = ParentBlue;
        }
    }
}
