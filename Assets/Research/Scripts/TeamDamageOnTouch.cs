using MLAgents.Policies;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts
{
    public class TeamDamageOnTouch : DamageOnTouch
    {
        public bool healingItem;
        private bool _isTeam;
        protected override void Colliding(GameObject collider)
        {
            if (Owner)
            {
                var owner = Owner.GetComponent<BehaviorParameters>();
                var character = collider.GetComponent<BehaviorParameters>();

                if (character && owner)
                {
                    _isTeam = owner.TeamId == character.TeamId;

                    if (!healingItem && _isTeam)
                    {
                        return;
                    }
                }
            }
            base.Colliding(collider);
        }

        protected override void OnCollideWithDamageable(Health health)
        {
            base.OnCollideWithDamageable(health);
            if (!_isTeam)
            {
                // we apply the damage to the thing we've collided with
                _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration);
                if (DamageTakenEveryTime + DamageTakenDamageable > 0)
                {
                    SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
                }
            }
        }
    }
}
