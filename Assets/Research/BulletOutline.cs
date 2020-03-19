using Research.Scripts;
using UnityEngine;

namespace Research
{
    public class BulletOutline : MonoBehaviour
    {
        public SpriteOutline outline;

        private bool ParentBlue
        {
            get
            {
                var owner = GetComponentInParent<TeamDamageOnTouch>()?.Owner;
                return owner != null && owner.GetComponentInChildren<SpriteOutline>().IsBlue;
            }
        }

        private void Start()
        {
            outline.IsBlue = ParentBlue;
        }
    }
}
