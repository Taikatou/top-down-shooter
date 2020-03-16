using System.Collections;
using System.Collections.Generic;
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

        public Character[] MLPrefabs;

        public Character[] priorMLPrefabs;

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

        public virtual void Restart()
        {
            if (ShouldDeleteCharacter())
            {
                foreach (var player in Players)
                {
                    player.Reset();
                    Destroy(player.gameObject);
                }
            }

            Initialization();
            
            InstantiatePlayableCharacters();

            SpawnMultipleCharacters();

            MMGameEvent.Trigger("Load");
        }

        protected override void InstantiatePlayableCharacters()
        {
            base.InstantiatePlayableCharacters();
            if (ShouldDeleteCharacter())
            {
                var blueTeam = _colourSwitch % 2;
                for (var i = 0; i < teamSize; i++)
                {
                    SpawnTeamPlayer(MLPrefabs, blueTeam);
                    SpawnTeamPlayer(priorMLPrefabs, blueTeam);
                }
                _colourSwitch++;
            }
        }

        protected virtual void SpawnTeamPlayer(Character [] characterPrefabs, int blueTeam)
        {
            var index = _random.Next(0, PlayerPrefabs.Length);
            var playerPrefab = characterPrefabs[index];
            
            var newPlayer = Instantiate (playerPrefab, _initialSpawnPointPosition, Quaternion.identity);
            newPlayer.name = playerPrefab.name;
            Players.Add(newPlayer);

            // Set outline
            var outline = newPlayer.GetComponentInChildren<SpriteOutline>();
            var teamId = newPlayer.GetComponent<BehaviorParameters>().TeamId;
            outline.IsBlue = blueTeam == teamId;
        }

        protected override bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }
        
        private int GetReward(int teamId)
        {
            var teamDeaths = GetTeamDeaths();
            var draw = teamDeaths[0] == teamDeaths[1];
            if (!draw)
            {
                var winningId = teamDeaths[0] > teamDeaths[1] ? 0 : 1;
                return winningId == teamId? 1: -1;
            }

            return 0;
        }

        protected override IEnumerator GameOver()
        {
            var agents = FindObjectsOfType<TopDownAgent>();

            var log = "";
            foreach (var agent in agents)
            {
                var teamId = agent.GetComponent<BehaviorParameters>().TeamId;
                var reward = GetReward(teamId);
                agent.SetReward(reward);
                agent.EndEpisode();

                log += reward + ", ";
            }
            Debug.Log("Reward: " + log);
            
            Restart();
            yield break;
        }
    }
}
