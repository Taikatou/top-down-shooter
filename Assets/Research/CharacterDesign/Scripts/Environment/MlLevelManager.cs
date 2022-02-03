using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Unity.Simulation.Games;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MlLevelManager : LevelManager
    {
        private static IEnumerable<EnvironmentInstance> Environments => FindObjectsOfType<EnvironmentInstance>();

        private Dictionary<Character, EnvironmentInstance> _characterEnvironmentMap;

        public static bool UnitySimulation = false;

        private Dictionary<Character, EnvironmentInstance> CharacterEnvironmentMap =>
            _characterEnvironmentMap ??= new Dictionary<Character, EnvironmentInstance>();

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
                StartEnvironment(-1);
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

        private void RefreshCharacterMap()
        {
            CharacterEnvironmentMap.Clear();
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
        }
        
        protected override void InstantiatePlayableCharacters()
        {
            RefreshCharacterMap();
            base.InstantiatePlayableCharacters();
        }

        public override void PlayerDead(Character playerCharacter)
        {
            if (playerCharacter == null)
            {
                return;
            }
            if (!CharacterEnvironmentMap.ContainsKey(playerCharacter))
            {
                RefreshCharacterMap();
            }
            CharacterEnvironmentMap[playerCharacter].OnPlayerDeath(playerCharacter);
        }
    }
}
