using System;
using System.Collections;
using MLAgents.Policies;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters.Tank
{
    public class ShieldAbility : BaseTankAbility
    {
        protected override void StartAbility()
        {
            StartCoroutine(StartInvulnerbility());
        }

        protected IEnumerator StartInvulnerbility()
        {
            var teamId = GetComponent<BehaviorParameters>().TeamId;

            var characters = FindObjectsOfType<BehaviorParameters>();
            var friend = Array.Find(characters, character => character.TeamId == teamId && character.gameObject != gameObject);

            var health = friend.GetComponent<Health>();

            health.Invulnerable = true;

            yield return new WaitForSeconds(5);

            health.Invulnerable = false;
        }

        protected override void UpdateAbility()
        {
            
        }
    }
}