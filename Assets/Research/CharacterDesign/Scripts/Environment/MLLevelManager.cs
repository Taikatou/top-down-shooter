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

        public void AddCharacter(Character ch, EnvironmentInstance env)
        {
            if (!CharacterEnvironmentMap.ContainsKey(ch))
            {
                CharacterEnvironmentMap.Add(ch, env);
            }
        }

        protected override void InstantiatePlayableCharacters()
        {
            base.InstantiatePlayableCharacters();
            foreach (var environment in environments)
            {
                environment.InstantiatePlayableCharacters();
                Players.AddRange(environment.mlPlayers);
            }
        }

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
