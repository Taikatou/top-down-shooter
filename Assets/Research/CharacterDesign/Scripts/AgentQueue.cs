using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.LevelDesign.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class AgentQueue : MonoBehaviour
    {
        public LevelCurriculum currentCurriculum;
        
        public Character[] mlCharacters;

        public Character[] priorMlCharacters;
        
        public Character[] dumbyCharacters;

        public AvailableCharacters availableCharacters;

        private AvailableCharacters smartPriorCharacters;
        
        private AvailableCharacters dumbPriorCharacters;

        private AvailableCharacters AvailablePriorCharacters
        {
            get
            {
                switch (currentCurriculum)
                {
                    case LevelCurriculum.AllActive:
                        return smartPriorCharacters;
                }
                return dumbPriorCharacters;
            }
        }

        public int numAgents = 4;

        private void Awake()
        {
            availableCharacters = new AvailableCharacters();
            smartPriorCharacters = new AvailableCharacters();
            dumbPriorCharacters = new AvailableCharacters();

            for (var i = 0; i < numAgents; i++)
            {
                SpawnCharacters(mlCharacters, availableCharacters);
                SpawnCharacters(priorMlCharacters, smartPriorCharacters);   
                SpawnCharacters(dumbyCharacters, dumbPriorCharacters);   
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
                AvailablePriorCharacters.ReturnCharacter(player);
                player.gameObject.SetActive(false);
            }
        }

        public Character PopRandomMlCharacter()
        {
            return availableCharacters.PopRandomCharacter();
        }
        
        public Character PopRandomPriorMlCharacter()
        {
            return AvailablePriorCharacters.PopRandomCharacter();
        }
    }
}
