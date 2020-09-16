using Research.LevelDesign.NuclearThrone;
using UnityEngine;

namespace Research.UI
{
    public class ScreenShot : MonoBehaviour
    {
        public NuclearThroneLevelGenerator generator;
        public void TakeScreenShot()
        {
            Debug.Log("Take screenshot");
            ScreenCapture.CaptureScreenshot(generator.mapSeed.ToString(), 4);
        }
    }
}
