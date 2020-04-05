using UnityEngine;

namespace Research.Scripts
{
    public class SpeedChanger : MonoBehaviour
    {
        public float scale;
        void Start()
        {
            Time.timeScale = scale;
        }
    }
}
