using System.Collections;
using MLAgents.Policies;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEditor.UI;
using UnityEngine;

namespace Research.Scripts
{
    public class MLLevelManager : GrasslandsMultiplayerLevelManager
    {
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
            foreach (var player in Players)
            {
                player.Reset();
                Destroy(player.gameObject);
            }

            Initialization();
            InstantiatePlayableCharacters();
            SpawnMultipleCharacters();
            
            MMGameEvent.Trigger("Load");
        }

        protected override bool GameOverCondition()
        {
            var teamDeaths = GetTeamDeaths();

            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }

        public enum GameEnding { Draw, Loss, Win }

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
