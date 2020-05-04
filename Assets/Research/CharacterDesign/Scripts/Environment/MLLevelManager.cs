using System.Collections;
using System.Collections.Generic;
using MLAgents;
using MLAgents.Policies;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.CharacterDesign.Scripts.SpawnPoints;
using Research.LevelDesign.NuclearThrone;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class MLLevelManager : GrasslandsMultiplayerLevelManager
    {
        public int teamSize = 2;

        public AgentQueue agentQueue;

        public DataLogger dataLogger;

        public IGetSpawnPoints spawnPoints;

        public NuclearThroneLevelGenerator levelGenerator;

        private int changeLevelMap = 100;

        private int _levelCounter;

        public int teamCount = 2;

        protected override void Awake()
        {
            base.Awake();
            UpdateSpawnPoints();
        }

        private void UpdateSpawnPoints()
        {
            if (spawnPoints != null)
            {
                SpawnPoints.Clear();
                foreach (var point in spawnPoints.Points)
                {
                    SpawnPoints.Add(point);
                }
            }
        }

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in Players)
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
            agentQueue.ReturnCharacters(Players);

            Initialization();
            
            InstantiatePlayableCharacters();

            SpawnMultipleCharacters();

            MMGameEvent.Trigger("Load");
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

                UpdateSpawnPoints();
            }
        }

        public virtual void Restart()
        {
            foreach (var player in Players)
            {
                var requester = player.GetComponent<DecisionRequester>();

                if (requester)
                {
                    Academy.Instance.AgentPreStep -= requester.MakeRequests;   
                }
            }
            
            ChangeLevelDesign();

            WaitForRestart();
        }

        protected override void InstantiatePlayableCharacters()
        {
            Players = new List<Character>();
            for (var i = 0; i < teamSize; i++)
            {
                var mlCharacter = agentQueue.PopRandomMlCharacter();
                SpawnTeamPlayer(mlCharacter, false);
                var priorMlCharacter = agentQueue.PopRandomPriorMlCharacter();
                SpawnTeamPlayer(priorMlCharacter, true);
            }
        }

        protected virtual void SpawnTeamPlayer(Character newPlayer, bool prior)
        {
            Players.Add(newPlayer);

            // Set outline
            var outline = newPlayer.GetComponentInChildren<SpriteOutline>();
            outline.IsBlue = prior;

            var health = newPlayer.GetComponent<MlHealth>();
            if (health)
            {
                health.Revive();
                Debug.Log(health.gameObject.name);
            }
        }

        protected override bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == teamCount || teamDeaths[1] == teamCount;
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

        protected override IEnumerator GameOver()
        {
            var winningTeamId = WinningTeam();

            var loggedData = new [] {0, 0};
            var loggedNames = new[] {new List<string>(), new List<string>(), };
            foreach (var player in Players)
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
