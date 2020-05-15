using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
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

        public int numAgents = 4;

        public EnabledCharactersComponent currentEnabledCharacters;
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

                //currentEnabledCharacters.availableCharacters.Remove(player);
            }
        }

        public Character PopRandomMlCharacter()
        {
            var character = availableCharacters.PopRandomCharacter();
            //currentEnabledCharacters.availableCharacters.Add(character);
            return character;
        }
        
        public Character PopRandomPriorMlCharacter()
        {
            var character = AvailablePriorCharacters.PopRandomCharacter();
            //currentEnabledCharacters.availableCharacters.Add(character);
            return character;
        }
    }
}
