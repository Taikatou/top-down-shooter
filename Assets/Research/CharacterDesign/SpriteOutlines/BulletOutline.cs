using Research.CharacterDesign.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.SpriteOutlines
{
    public class BulletOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<TeamDamageOnTouch>()?.Owner;
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().isPrior;
            }
        }

        private void Start()
        {
            outline.isPrior = ParentBlue;
        }
    }
}
