using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone;
using Research.LevelDesign.Scripts;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public sealed class EnvironmentInstance : MonoBehaviour
    {
        public int teamSize = 2;
        
        public DataLogger dataLogger;

        public NuclearThroneLevelGenerator levelGenerator;

        public float gameTime = 60;
        
        public int changeLevelMap = 10;

        private int _levelCounter = 0;

        public int CurrentLevelCounter => _levelCounter;

        public GetSpawnProcedural getSpawnProcedural;

        private MLCheckbox[] SpawnPoints => getSpawnProcedural.Points;

        public EntityMapPosition[] EntityMapPositions => GetComponentsInChildren<EntityMapPosition>();

        public Character[] mlCharacters;
        
        private float _timer;

        public int CurrentTimer => (int)_timer;

        private bool _gameOver;

        private void Start()
        {
            _timer = gameTime;
        }

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in mlCharacters)
            {
                if (MlUtils.Dead(character))
                {
                    var behaviour = character.GetComponent<BehaviorParameters>();
                    var index = behaviour.TeamId;
                    teamDeaths[index]++;
                }
            }
            return teamDeaths;
        }

        private void WaitForRestart()
        {
            InstantiatePlayableCharacters();

            SpawnMultipleCharacters();

            _gameOver = false;
            
            _timer = gameTime;
        }

        public void SpawnMultipleCharacters()
        {
            for (var i = 0; i < mlCharacters.Length; i++)
            {
                SpawnPoints[i].SpawnPlayer(mlCharacters[i]);
            }
        }

        private void ChangeLevelDesign()
        {
            if (levelGenerator)
            {
                _levelCounter++;
                if (_levelCounter == changeLevelMap)
                {
                    levelGenerator.GenerateMap();
                    _levelCounter = 0;
                }
            }
        }

        public void Restart()
        {
            ChangeLevelDesign();

            WaitForRestart();
        }

        public void InstantiatePlayableCharacters()
        {
            foreach(var agent in mlCharacters)
            {
                SpawnTeamPlayer(agent);
            }
        }

        private void SpawnTeamPlayer(Character newPlayer)
        {
            var health = newPlayer.GetComponent<MlHealth>();
            health.Revive();
        }
        
        public void OnPlayerDeath(Character playerCharacter)
        {
            var gameOver = GameOverCondition();
            if (gameOver)
            {
                StartCoroutine(GameOver());
            }
        }
        
        private bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == teamSize || teamDeaths[1] == teamSize;
            return gameOver;
        }

        private int WinningTeam()
        {
            var teamDeaths = GetTeamDeaths();
            var draw = teamDeaths[0] == teamDeaths[1];
            if (!draw)
            {
                return teamDeaths[0] < teamDeaths[1] ? 0 : 1;
            }

            return -1;
        }
        
        private int GetReward(int teamId, int winningTeam)
        {
            if(winningTeam != -1)
            {
                return winningTeam == teamId? 1: -1;
            }

            return 0;
        }

        private void Update()
        {
            if (!_gameOver)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _gameOver = true;
                    StartCoroutine(GameOver());
                }   
            }
        }

        private IEnumerator GameOver()
        {
            var winningTeamId = WinningTeam();

            var loggedData = new [] {0, 0};
            var loggedNames = new[] {new List<string>(), new List<string>(), };
            foreach (var player in mlCharacters)
            {
                var behaviour = player.GetComponentInChildren<BehaviorParameters>();
                var agentName = behaviour.FullyQualifiedBehaviorName;
                var teamId = behaviour.TeamId;
                var reward = GetReward(teamId, winningTeamId);
                
                loggedData[teamId] = reward;
                loggedNames[teamId].Add(agentName);

                var agent = player.GetComponentInChildren<TopDownAgent>();
                agent.AddReward(reward);
                agent.EndEpisode();
            }

            loggedNames[0].Sort();
            loggedNames[1].Sort();
            var sortedNames0 = string.Join("_", loggedNames[0].ToArray());
            var sortedNames1 = string.Join("_", loggedNames[1].ToArray());

            if (dataLogger != null)
            {
                dataLogger.AddResultTeam(sortedNames0, loggedData[0]);
                dataLogger.AddResultTeam(sortedNames1, loggedData[1]);
            }
            Debug.Log("Winning Team" + winningTeamId);
            Debug.Log( "Team 0: " + sortedNames0 + " reward: " + loggedData[0] + 
                       "Team 1: " + sortedNames1 + " reward: " + loggedData[1]);

            Restart();
            yield break;
        }
    }
}
