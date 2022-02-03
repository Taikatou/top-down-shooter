using System;
using System.Collections;
using Characters.Tank;
using MoreMountains.TopDownEngine;
using Research.LevelDesign.Scripts;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters.Tank
{
    public class ShieldAbility : BaseTankAbility
    {
        [Observable] 
        private bool _invulnerble = false;
        
        protected override void StartAbility()
        {
            StartCoroutine(StartInvulnerbility());
        }

        private IEnumerator StartInvulnerbility()
        {
            var teamId = GetComponent<BehaviorParameters>().TeamId;

            //TODO FIX THIS FIND OBJECTS OF TYPE
            var getter = GetComponentInParent<CharacterGetter>();
            
            var characters = getter.GetComponentsInChildren<BehaviorParameters>();
            var friend = Array.Find(characters, character => character.TeamId == teamId && character.gameObject != gameObject);

            var health = friend.GetComponentInParent<Health>();
            
            _invulnerble = true;
            health.Invulnerable = _invulnerble;

            yield return new WaitForSeconds(5);

            _invulnerble = false;
            health.Invulnerable = _invulnerble;
        }

        protected override void UpdateAbility()
        {
            
        }
    }
}