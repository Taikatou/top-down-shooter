using System;
using System.Collections;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class ShieldAbility : BaseTankAbility
    {
        protected override void StartAbility()
        {
            Debug.Log("Start");
            StartCoroutine(StartInvulnerbility());
        }

        protected IEnumerator StartInvulnerbility()
        {
            var teamId = GetComponent<Character>().TeamId;

            var characters = FindObjectsOfType<Character>();
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
