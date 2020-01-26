using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class AOEHeal : MonoBehaviour
    {
        private Collider2D[] colliders;

        public int HealDistance = 3;

        public float HealPerSecond = 5;

        // Update is called once per frame
        void Update()
        {
            var healthBack = HealPerSecond * Time.deltaTime;
            var position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            colliders = Physics2D.OverlapCircleAll(position, HealDistance);
            foreach (var t in colliders)
            {
                HealUnit(t.gameObject, healthBack);
            }
        }

        public void HealUnit(GameObject unitToHeal, float healthBack)
        {
            var health = unitToHeal.GetComponent<Health>();
            if (health)
            {
                Debug.Log("healing: " + unitToHeal);
                health.GetHealth(healthBack, gameObject);
            }
        }
    }
}
