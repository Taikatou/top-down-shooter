using UnityEngine;

namespace Research.CharacterDesign.Scripts
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
