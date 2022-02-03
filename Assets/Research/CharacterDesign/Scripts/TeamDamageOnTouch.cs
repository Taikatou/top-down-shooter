using MoreMountains.TopDownEngine;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.Scripts.MLAgents;
using Unity.MLAgents;
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
                var owner = Owner.GetComponent<GetTeamID>();
                var character = collider.GetComponent<GetTeamID>();
                
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

        private void RewardAgent(float reward)
        {
            if (TrainingConfig.RewardShotSuccess)
            {
                Owner.GetComponentInChildren<Agent>().AddReward(reward);
            }
        }

        protected override void OnCollideWithDamageable(Health health)
        {
            if (!_isTeam)
            {
                // we apply the damage to the thing we've collided with
                _colliderHealth.Damage(DamageCaused, gameObject, InvincibilityDuration, InvincibilityDuration);
                RewardAgent(0.1f);
            }
            else if(healingItem)
            {
                _colliderHealth.GetHealth(healCaused, gameObject);
                RewardAgent(0.05f);
            }

            if (DamageTakenEveryTime + DamageTakenDamageable > 0)
            {
                SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
            }
        }
    }
}
