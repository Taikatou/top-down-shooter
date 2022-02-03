using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.UI;

namespace Research.UI
{
    public class HealthUI : MonoBehaviour
    {
        public Text textComponent;

        public int playerId;

        private void Update()
        {
          //  var characters = FindObjectsOfType<MlCharacter>();
          //  var health = Array.Find(characters, character => character.GetComponent<BehaviorParameters>().TeamId+ 1 == playerId).GetComponent<Health>();
          //  textComponent.text = "Player: " + playerId + "\t(" + health.CurrentHealth + "/" + health.MaximumHealth + ")";
        }
    }
}
