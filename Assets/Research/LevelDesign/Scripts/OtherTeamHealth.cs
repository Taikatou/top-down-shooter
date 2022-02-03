using System;
using MoreMountains.TopDownEngine;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Research.LevelDesign.Scripts
{
    public class OtherTeamHealth : MonoBehaviour
    {
        [Observable]
        private float _otherHealth = 1;

        private int _teamId;

        private void Start()
        {
            _teamId = GetComponent<BehaviorParameters>().TeamId;
        }

        private void Update()
        {
            var getter = GetComponentInParent<CharacterGetter>();
            
            var characters = getter.GetComponentsInChildren<BehaviorParameters>();
            var friend = Array.Find(characters, character => character.TeamId == _teamId && character.gameObject != gameObject);
            
            var health = friend.GetComponentInParent<Health>();

            _otherHealth = (float)health.CurrentHealth / (float)health.MaximumHealth;
        }
    }
}
