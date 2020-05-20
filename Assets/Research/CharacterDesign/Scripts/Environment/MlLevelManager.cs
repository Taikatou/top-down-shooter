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
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MlLevelManager : LevelManager
    {
        private static IEnumerable<EnvironmentInstance> Environments => FindObjectsOfType<EnvironmentInstance>();

        private Dictionary<Character, EnvironmentInstance> _characterEnvironmentMap;

        private Dictionary<Character, EnvironmentInstance> CharacterEnvironmentMap =>
            _characterEnvironmentMap ??
            (_characterEnvironmentMap = new Dictionary<Character, EnvironmentInstance>());

        protected override void SpawnSingleCharacter()
        {
            SpawnCharacters();
        }

        protected override void SpawnMultipleCharacters()
        {
            SpawnCharacters();
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
