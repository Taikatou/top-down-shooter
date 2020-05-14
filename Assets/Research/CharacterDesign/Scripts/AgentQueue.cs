using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.Common;
using Research.Common.MapSensor.SensorComponent;
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

        public int numAgents = 4;

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
            AvailableCharacters = new List<GameObject>();

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
                var newAgent = newPlayer.GetComponentsInChildren<TileMapSensorComponent>();

                foreach (var sensor in newAgent)
                {
                    sensor.LearningEnvironment = GetComponent<LearningEnvironmentAccessor>().learningEnvironment;
                }
                
                var agent = newPlayer.GetComponent<TopDownAgent>();
                if (agent)
                {
                    agent.groundRb = groundRb;
                }
                
                available.Add(newPlayer);
                newPlayer.gameObject.SetActive(false);
            }
        }

        public List<GameObject> AvailableCharacters { get; private set; }

        public void ReturnCharacters(IEnumerable<Character> players)
        {
            foreach (var player in players)
            {
                player.Reset();
                availableCharacters.ReturnCharacter(player);
                AvailablePriorCharacters.ReturnCharacter(player);
                player.gameObject.SetActive(false);

                AvailableCharacters.Remove(player.gameObject);
            }
        }

        public Character PopRandomMlCharacter()
        {
            var character = availableCharacters.PopRandomCharacter();
            AvailableCharacters.Add(character.gameObject);
            return character;
        }
        
        public Character PopRandomPriorMlCharacter()
        {
            var character = AvailablePriorCharacters.PopRandomCharacter();
            AvailableCharacters.Add(character.gameObject);
            return character;
        }
    }
}
