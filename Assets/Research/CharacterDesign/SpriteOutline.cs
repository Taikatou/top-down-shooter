using UnityEngine;

// Outline is based from https://nielson.dev/2016/04/2d-sprite-outlines-in-unity
namespace Research
{
    [ExecuteInEditMode]
    public class SpriteOutline : MonoBehaviour
    {
        public bool isPrior;
        private Color Color => isPrior ? Color.white : Color.black;

        [Range(0, 16)]
        public int outlineSize = 1;

        private SpriteRenderer spriteRenderer;

        void OnEnable() {
            spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateOutline(true);
        }

        void OnDisable() {
            UpdateOutline(false);
        }

        void Update() {
            UpdateOutline(true);
        }

        void UpdateOutline(bool outline) {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Outline", outline ? 1f : 0);
            mpb.SetColor("_OutlineColor", Color);
            mpb.SetFloat("_OutlineSize", outlineSize);
            spriteRenderer.SetPropertyBlock(mpb);
        }
    }
}