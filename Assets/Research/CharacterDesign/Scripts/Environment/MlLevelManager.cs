using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.Common;
using Research.Common.EntitySensor;
using Research.LevelDesign.NuclearThrone;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.Simulation.Games;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MlLevelManager : LevelManager
    {
        private static IEnumerable<EnvironmentInstance> Environments => FindObjectsOfType<EnvironmentInstance>();

        private Dictionary<Character, EnvironmentInstance> _characterEnvironmentMap;
        
        public static readonly bool UnitySimulation = false;

        private Dictionary<Character, EnvironmentInstance> CharacterEnvironmentMap =>
            _characterEnvironmentMap ??
            (_characterEnvironmentMap = new Dictionary<Character, EnvironmentInstance>());

        protected override void Start()
        {
            base.Start();
            Application.targetFrameRate = 60;
        }

        protected override void SpawnSingleCharacter()
        {
            // SpawnCharacters();
            GetConfigSimulation();
        }

        protected override void SpawnMultipleCharacters()
        {
            // SpawnCharacters();
            GetConfigSimulation();
        }

        private void GetConfigSimulation()
        {
            if (UnitySimulation)
            {
                GameSimManager.Instance.FetchConfig(OnConfigReceived);   
            }
            else
            {
                var random = (int) System.DateTime.Now.Ticks;
                StartEnvironment(random);
            }
        }
        
        private void OnConfigReceived(GameSimConfigResponse config)
        {
            var random = config.GetInt("random_seed");
            StartEnvironment(random);
        }

        private void StartEnvironment(int random)
        {
            foreach (var environment in Environments)
            {
                environment.StartSimulation(random);
            }
        }

        private void SpawnCharacters()
        {
            foreach (var environment in Environments)
            {
                environment.SpawnMultipleCharacters();
            }
        }

        protected override void InstantiatePlayableCharacters()
        {
            foreach (var environment in Environments)
            {
                foreach (var player in environment.mlCharacters)
                {
                    SceneCharacters.Add(player);
                    if (!CharacterEnvironmentMap.ContainsKey(player))
                    {
                        CharacterEnvironmentMap.Add(player, environment);
                    }
                    else
                    {
                        Debug.Log("Invalid stuff");
                    }
                }
            }

            base.InstantiatePlayableCharacters();
        }

        public override void PlayerDead(Character playerCharacter)
        {
            if (playerCharacter == null)
            {
                return;
            }
            var characterHealth = playerCharacter.GetComponent<Health>();
            if (characterHealth != null && CharacterEnvironmentMap.ContainsKey(playerCharacter))
            {
                CharacterEnvironmentMap[playerCharacter].OnPlayerDeath(playerCharacter);
            }
        }
    }
}
