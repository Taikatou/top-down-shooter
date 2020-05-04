using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.LevelDesign.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class AgentQueue : MonoBehaviour
    {
        public Character[] mlCharacters;

        public Character[] priorMlCharacters;

        public AvailableCharacters availableCharacters;
        
        public AvailableCharacters availablePriorCharacters;

        public int numAgents = 4;

        private void Awake()
        {
            availableCharacters = new AvailableCharacters();
            availablePriorCharacters = new AvailableCharacters();

            for (var i = 0; i < numAgents; i++)
            {
                SpawnCharacters(mlCharacters, availableCharacters);
                SpawnCharacters(priorMlCharacters, availablePriorCharacters);   
            }
        }

        private void SpawnCharacters(Character[] characters, AvailableCharacters available)
        {
            foreach (var playerPrefab in characters)
            {
                var newPlayer = Instantiate(playerPrefab, gameObject.transform.position, Quaternion.identity);
                newPlayer.transform.parent = gameObject.transform;
                available.Add(newPlayer);
                newPlayer.gameObject.SetActive(false);
            }
        }

        public void ReturnCharacters(List<Character> players)
        {
            foreach (var player in players)
            {
                player.Reset();
                availableCharacters.ReturnCharacter(player);
                availablePriorCharacters.ReturnCharacter(player);
                player.gameObject.SetActive(false);
            }
        }

        public Character PopRandomMlCharacter()
        {
            return availableCharacters.PopRandomCharacter();
        }
        
        public Character PopRandomPriorMlCharacter()
        {
            return availablePriorCharacters.PopRandomCharacter();
        }
    }
}
