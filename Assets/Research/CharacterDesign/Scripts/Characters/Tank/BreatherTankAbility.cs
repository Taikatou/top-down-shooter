using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Characters.Tank
{
    public class BreatherTankAbility : BaseTankAbility
    {
        public float breathTime;

        public float heathBack;
        private Health HealthComponent => GetComponent<Health>();
        protected override void StartAbility()
        {
            if (HealthComponent.CurrentHealth < HealthComponent.MaximumHealth)
            {
                BreathButtonDown = true;
                CurrentlyBreathing = true;
                CurrentTime = breathTime;
                SetWeaponsEnabled(false);

                _used = true;
            }
        }

        protected override void UpdateAbility()
        {
            var healthBack = (heathBack / breathTime) * Time.deltaTime;
            if (HealthComponent)
            {
                HealthComponent.GetHealth(healthBack, gameObject);
            }
        }
    }
}
