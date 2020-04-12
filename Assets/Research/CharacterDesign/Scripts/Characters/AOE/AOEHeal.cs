using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class AOEHeal : MonoBehaviour
    {
        private Collider2D[] colliders;

        public int HealDistance = 3;

        public float HealPerSecond = 5;

        public int healedCharachters;

        public float healthRecovered;
        // Update is called once per frame
        private void FixedUpdate()
        {
            var healthBack = HealPerSecond * Time.deltaTime;
            var position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            colliders = Physics2D.OverlapCircleAll(position, HealDistance);
            healedCharachters = 0;
            healthRecovered = 0.0f;
            foreach (var t in colliders)
            {
                HealUnit(t.gameObject, healthBack);
            }
        }

        public void HealUnit(GameObject unitToHeal, float healthBack)
        {
            var health = unitToHeal.GetComponent<Health>();
            if (health != null && health.CurrentHealth > 0)
            {
                healedCharachters++;
                healthRecovered += health.GetHealth(healthBack, gameObject);
            }
        }

        public Dictionary<string, float> GetObservations()
        {
            var obs = new Dictionary<string, float>
            {
                { "Healed Characters", healedCharachters },
                { "Health Recovered", healthRecovered }
            };

            return obs;
        }

        public string SenseName => "AOEHeal";
    }
}
