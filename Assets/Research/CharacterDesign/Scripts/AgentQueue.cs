using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.Common;
using Research.LevelDesign.Scripts;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class AgentQueue : MonoBehaviour
    {
        public LevelCurriculum currentCurriculum;
        
        public Character[] mlCharacters;

        public Character[] priorMlCharacters;
        
        public Character[] dumbCharacters;

        public AvailableCharacters availableCharacters;

        private AvailableCharacters _smartPriorCharacters;
        
        private AvailableCharacters _dumbPriorCharacters;

        public Rigidbody2D groundRb;

        private AvailableCharacters AvailablePriorCharacters
        {
            get
            {
                switch (currentCurriculum)
                {
                    case LevelCurriculum.AllActive:
                        return _smartPriorCharacters;
                }
                return _dumbPriorCharacters;
            }
        }

        public int numAgents = 4;

        private void Awake()
        {
            availableCharacters = new AvailableCharacters();
            _smartPriorCharacters = new AvailableCharacters();
            _dumbPriorCharacters = new AvailableCharacters();

            for (var i = 0; i < numAgents; i++)
            {
                SpawnCharacters(mlCharacters, availableCharacters);
                SpawnCharacters(priorMlCharacters, _smartPriorCharacters);   
                SpawnCharacters(dumbCharacters, _dumbPriorCharacters);   
            }
        }

        private void SpawnCharacters(IEnumerable<Character> characters, AvailableCharacters available)
        {
            foreach (var playerPrefab in characters)
            {
                var newPlayer = Instantiate(playerPrefab, gameObject.transform.position, Quaternion.identity);
                newPlayer.transform.parent = gameObject.transform;
                
                var agent = newPlayer.GetComponent<TopDownAgent>();
                if (agent)
                {
                    agent.groundRb = groundRb;
                }
                
                available.Add(newPlayer);
                newPlayer.gameObject.SetActive(false);
            }
        }

        public void ReturnCharacters(IEnumerable<Character> players)
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
