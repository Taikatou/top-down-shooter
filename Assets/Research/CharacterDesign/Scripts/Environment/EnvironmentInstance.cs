using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
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
        
        private const int ChangeLevelMap = 100;

        private int _levelCounter;

        public int CurrentLevelCounter => _levelCounter;

        public GetSpawnProcedural getSpawnProcedural;

        private List<MLCheckbox> SpawnPoints => getSpawnProcedural.Points;

        public List<Character> mlPlayers;
        
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
            foreach (var character in mlPlayers)
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
            for (var i = 0; i < mlPlayers.Count; i++)
            {
                SpawnPoints[i].SpawnPlayer(mlPlayers[i]);
            }
        }

        private void ChangeLevelDesign()
        {
            if (levelGenerator)
            {
                _levelCounter++;
                if (_levelCounter == ChangeLevelMap)
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
            foreach(var agent in mlPlayers)
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
            foreach (var player in mlPlayers)
            {
                var agent = player.GetComponent<TopDownAgent>();
                var teamId = agent.GetComponent<BehaviorParameters>().TeamId;
                var reward = GetReward(teamId, winningTeamId);
                
                loggedData[teamId] = reward;
                var agentName = agent.GetComponent<BehaviorParameters>().FullyQualifiedBehaviorName;
                loggedNames[teamId].Add(agentName);

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
