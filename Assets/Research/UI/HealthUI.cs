using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Research.UI
{
    public class HealthUI : MonoBehaviour
    {
        public Text textComponent;

        public Health health;

        public int playerId;

        private void Update()
        {
            textComponent.text = "Player: " + playerId + "\t(" + health.CurrentHealth + "/" + health.MaximumHealth + ")";
        }
    }
}
