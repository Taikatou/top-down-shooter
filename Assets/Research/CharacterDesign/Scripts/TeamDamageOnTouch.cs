using MoreMountains.TopDownEngine;
using Research.Common.MapSensor.GridSpaceEntity;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class TeamDamageOnTouch : DamageOnTouch
    {
        public bool healingItem;
        private bool _isTeam;

        public float healCaused;

        protected override void Colliding(GameObject collider)
        {
            if (Owner)
            {
                var owner = Owner.GetComponent<IGetTeamId>();
                var character = collider.GetComponent<IGetTeamId>();

                if (character && owner)
                {
                    _isTeam = owner.GetTeamId == character.GetTeamId;

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
            if (!_isTeam)
            {
                // we apply the damage to the thing we've collided with
                _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration);
            }
            else
            {
                _colliderHealth.GetHealth(healCaused, gameObject);
            }
            
            if (DamageTakenEveryTime + DamageTakenDamageable > 0)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }
    }
}
