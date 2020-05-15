using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.Common;
using Research.Common.EntitySensor;
using Research.LevelDesign.NuclearThrone;
using SpawnPoints;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MlLevelManager : LevelManager
    {
        public List<EnvironmentInstance> environments;

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
            foreach (var environment in environments)
            {
                environment.SpawnMultipleCharacters();
            }
        }

        protected override void InstantiatePlayableCharacters()
        {
            foreach (var environment in environments)
            {
                foreach (var player in environment.mlPlayers)
                {
                    SceneCharacters.Add(player);
                    if (!CharacterEnvironmentMap.ContainsKey(player))
                    {
                        CharacterEnvironmentMap.Add(player, environment);
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
