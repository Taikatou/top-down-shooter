using Research.LevelDesign.NuclearThrone;
using UnityEngine;

namespace Research.UI
{
    public class ScreenShot : MonoBehaviour
    {
        public NuclearThroneLevelGenerator generator;
        void OnMouseDown()
        {
            ScreenCapture.CaptureScreenshot(generator.MapSeed.ToString());
        }
    }
}
