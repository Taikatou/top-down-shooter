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
    public enum WinLossCondition { Win, Loss, Draw};
    public sealed class EnvironmentInstance : GetEnvironmentMapPositions
    {
        public float gameTime = 120;
        private float _timer;

        public int teamSize = 2;
        public int changeLevelMap = 10;
        
        public AnalysisTool outPutter;
        public NuclearThroneLevelGenerator levelGenerator;
        public GetSpawnProcedural getSpawnProcedural;
        public Character[] mlCharacters;

        
        public int CurrentLevelCounter { get; private set; }

        private int _randomSeed;
        private bool _gameOver;

        public override EntityMapPosition[] EntityMapPositions => GetComponentsInChildren<EntityMapPosition>();

        private static readonly Dictionary<WinLossCondition, int> RewardMap = new Dictionary<WinLossCondition, int>
        {
            { WinLossCondition.Win, 1 },
            { WinLossCondition.Draw, 0},
            { WinLossCondition.Loss, -1}
        };

        private void Start()
        {
            _timer = gameTime;
            CurrentLevelCounter = changeLevelMap;
        }

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in mlCharacters)
            {
                if (MlUtils.Dead(character))
                {
                    var behaviour = character.GetComponentInChildren<BehaviorParameters>();
                    var index = behaviour.TeamId;
                    teamDeaths[index]++;
                }
            }
            return teamDeaths;
        }

        public void SpawnMultipleCharacters()
        {
            for (var i = 0; i < mlCharacters.Length; i++)
            {
                var spawnPoint = getSpawnProcedural.PointDict[i];
                spawnPoint.SpawnPlayer(mlCharacters[i]);
                // Debug.Assert([i], "Agent component was not found on this gameObject and is required.");
            }
        }

        private void ChangeLevelDesign()
        {
            if (levelGenerator)
            {
                if (CurrentLevelCounter >= changeLevelMap)
                {
                    levelGenerator.GenerateMap(_randomSeed);
                    CurrentLevelCounter = 1;
                }
                CurrentLevelCounter++;
            }
        }

        public void StartSimulation(int randomSeed)
        {
            _randomSeed = randomSeed * GetHashCode();
            StartCoroutine(Restart());
        }

        public override IEnumerator Restart()
        {
            // Pause AI and start stuff
            SetAllowDecisions(false);
            yield return new WaitForEndOfFrame();
            
            ChangeLevelDesign();
            // Restart the game
            InstantiatePlayableCharacters();
            SpawnMultipleCharacters();

            // reenable characters
            SetAllowDecisions(true);
            
            _timer = gameTime;
            _gameOver = false;
        }

        private void SetAllowDecisions(bool allow)
        {
            foreach (var agent in mlCharacters)
            {
                var decisionRequester = agent.GetComponentInChildren<DecisionRequester>();
                decisionRequester.allowDecisions = allow;
            }
        }

        private void InstantiatePlayableCharacters()
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

        private WinLossCondition GetRewardCondition(int teamId, int winningTeam)
        {
            if(winningTeam != -1)
            {
                return winningTeam == teamId? WinLossCondition.Win: WinLossCondition.Loss;
            }
            return WinLossCondition.Draw;
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
            var loggedNames = new List<string>();
            foreach (var player in mlCharacters)
            {
                var behaviour = player.GetComponentInChildren<BehaviorParameters>();
                var agentName = behaviour.FullyQualifiedBehaviorName;
                var teamId = behaviour.TeamId;
                var winLossCondition = GetRewardCondition(teamId, winningTeamId);
                var reward = RewardMap[winLossCondition];
                
                loggedData[teamId] = reward;
                loggedNames.Add(agentName);

                var agent = player.GetComponentInChildren<TopDownAgent>();
                agent.AddReward(reward);
                agent.EndEpisode();

                Debug.Log(winLossCondition + "\t" + agentName);
                if (teamId == 0)
                {
                    outPutter.AddResult(winLossCondition);
                }
            }

            Debug.Log("Winning Team" + winningTeamId);
            Debug.Log(loggedNames[0] + " reward: " + loggedData[0] + "\n" + 
                      loggedNames[1] + " reward: " + loggedData[1] + "\n");

            if (MlLevelManager.UnitySimulation)
            {
                Application.Quit();
            }
            else
            {
                StartCoroutine(Restart());   
            }
            yield break;
        }
    }
}
