using System.Collections;
using System.Collections.Generic;
using MLAgents;
using MLAgents.Policies;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts.Environment
{
    public class MLLevelManager : GrasslandsMultiplayerLevelManager
    {
        private int _turnCounter = 0;

        private int _colourSwitch = 0;

        public int teamSize = 2;

        private System.Random _random;

        public Character[] mlCharacters;

        public Character[] priorMlCharacters;

        protected override void Start()
        {
            _random = new System.Random();
            base.Start();
        }

        protected virtual bool ShouldDeleteCharacter()
        {
            return _turnCounter % 10 == 0;
        }
        
        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in Instance.Players)
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

        protected IEnumerator WaitForRestart()
        {
            yield return new WaitForSeconds(1);
            _turnCounter++;
            
            var shouldDelete = ShouldDeleteCharacter();
            foreach (var player in Players)
            {
                player.Reset();
                if (shouldDelete)
                {
                    Destroy(player.gameObject);
                }
            }

            Initialization();

            if (shouldDelete)
            {
                Debug.Log("Instantiate now");
                InstantiatePlayableCharacters();
            }

            SpawnMultipleCharacters();

            MMGameEvent.Trigger("Load");
        }

        public virtual void Restart()
        {
            foreach (var player in Players)
            {
                var requester = player.GetComponent<DecisionRequester>();
            
                requester.allowDecisions = false;
            }
            
            StartCoroutine(WaitForRestart());
        }

        protected override void InstantiatePlayableCharacters()
        {
            Players = new List<Character>();
            var blueTeam = _colourSwitch % 2;
            for (var i = 0; i < teamSize; i++)
            {
                SpawnTeamPlayer(mlCharacters, blueTeam, false);
                SpawnTeamPlayer(priorMlCharacters, blueTeam, true);
            }
            _colourSwitch++;
        }

        protected virtual void SpawnTeamPlayer(Character [] characterPrefabs, int blueTeam, bool prior)
        {
            var index = _random.Next(0, PlayerPrefabs.Length);
            var playerPrefab = characterPrefabs[index];
            
            var newPlayer = Instantiate (playerPrefab, _initialSpawnPointPosition, Quaternion.identity);
            newPlayer.name = playerPrefab.name;
            Players.Add(newPlayer);
            
            // Set TeamId
            var behaviour = newPlayer.GetComponent<BehaviorParameters>();
            // behaviour.TeamId = prior? 0 : 1;

            // Set outline
            var outline = newPlayer.GetComponentInChildren<SpriteOutline>();
            var teamId = behaviour.TeamId;
            outline.IsBlue = blueTeam == teamId;
        }

        protected override bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }

        private int WinningTeam()
        {
            var teamDeaths = GetTeamDeaths();
            var draw = teamDeaths[0] == teamDeaths[1];
            if (!draw)
            {
                return teamDeaths[0] > teamDeaths[1] ? 0 : 1;
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
            var agents = FindObjectsOfType<TopDownAgent>();
            var winningTeamId = WinningTeam();
            
            var log = "";
            foreach (var agent in agents)
            {
                var teamId = agent.GetComponent<BehaviorParameters>().TeamId;
                var reward = GetReward(teamId, winningTeamId);
                agent.AddReward(reward);
                agent.EndEpisode();

                log += reward + ", ";
            }
            Debug.Log("Winning Team Id: " + winningTeamId + "\tReward: " + log);
            
            Restart();
            yield break;
        }
    }
}
