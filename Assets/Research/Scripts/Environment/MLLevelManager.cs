using System.Collections;
using System.Collections.Generic;
using MLAgents.Policies;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

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
                if (Dead(character))
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
            Players = new List<Character> ();
            
            if (ShouldDeleteCharacter())
            {
                var blueTeam = _colourSwitch % 2;
                for (var i = 0; i < teamSize; i++)
                {
                    SpawnTeamPlayer(mlCharacters, blueTeam, false);
                    SpawnTeamPlayer(priorMlCharacters, blueTeam, true);
                }
                _colourSwitch++;
            }
        }

        protected virtual void SpawnTeamPlayer(Character [] characters, int blueTeam, bool team2)
        {
            var index = _random.Next(0, PlayerPrefabs.Length);
            var playerPrefab = characters[index];
            
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

        private enum GameEnding { Draw, Loss, Win }

        private GameEnding IsWinner(TopDownAgent agent)
        {
            var behaviour = agent.GetComponent<BehaviorParameters>();

            var teamDeaths = GetTeamDeaths();
            
            Debug.Log(teamDeaths[0] + "\t" + teamDeaths[1]);
            if ((teamDeaths[0] > 0 || teamDeaths[1] > 0) && teamDeaths[0] != teamDeaths[1])
            {
                var winningId = teamDeaths[0] > teamDeaths[1] ? 1 : 2;
                var winner = behaviour.TeamId == winningId;
                return winner ? GameEnding.Win : GameEnding.Loss;
            }

            return GameEnding.Draw;
        }

        protected override IEnumerator GameOver()
        {
            var agents = FindObjectsOfType<TopDownAgent>();

            foreach (var agent in agents)
            {
                var winner = IsWinner(agent);
                if (winner == GameEnding.Win)
                {
                    agent.AddReward(1.0f);
                }
                else if (winner == GameEnding.Loss)
                {
                    agent.AddReward(-0.25f);
                }

                agent.EndEpisode();
            }

            Debug.Log("GameOver");
            Restart();
            yield break;
        }
    }
}
