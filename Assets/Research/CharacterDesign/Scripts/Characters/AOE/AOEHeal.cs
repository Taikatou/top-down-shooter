using MoreMountains.TopDownEngine;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Characters
{
    public class AOEHeal : MonoBehaviour
    {
        private Collider2D[] _colliders;

        public int HealDistance = 3;

        public float HealPerSecond = 5;

        [Observable]
        public int healedCharacters;

        [Observable]
        public float healthRecovered;
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            var healthBack = HealPerSecond * Time.deltaTime;
            var position1 = transform.position;
            var position = new Vector3(position1.x, position1.y + 0.3f, position1.z);
            _colliders = Physics2D.OverlapCircleAll(position, HealDistance);
            healedCharacters = 0;
            healthRecovered = 0.0f;
            foreach (var t in _colliders)
            {
                HealUnit(t.gameObject, healthBack);
            }
        }

        private void HealUnit(GameObject unitToHeal, float healthBack)
        {
            var health = unitToHeal.GetComponent<Health>();
            if (health != null && health.CurrentHealth > 0)
            {
                healedCharacters++;
                healthRecovered += health.GetHealth(healthBack, gameObject);
            }
        }
    }
}
