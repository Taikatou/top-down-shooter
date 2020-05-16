using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;
using UnityEngine.UI;

namespace Research.UI
{
    public class LevelCounterUI : MonoBehaviour
    {
        public Text textComponent;

        public EnvironmentInstance instance;

        private void Update()
        {
            textComponent.text = instance.CurrentLevelCounter.ToString();
        }
    }
}
