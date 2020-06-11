using System.Globalization;
using Research.CharacterDesign.Scripts.Environment;
using UnityEngine;
using UnityEngine.UI;

namespace Research.UI
{
    public class CountDownTimer : MonoBehaviour
    {
        public Text textComponent;

        public EnvironmentInstance instance;

        private void Update()
        {
            textComponent.text = instance.CurrentTimer.ToString(CultureInfo.InvariantCulture);
        }
    }
}
