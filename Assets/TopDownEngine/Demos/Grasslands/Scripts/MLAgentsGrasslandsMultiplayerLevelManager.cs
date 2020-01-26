using System.Collections;
using System.Linq;
using BattleResearch.Scripts;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngine.Demos.Grasslands.Scripts
{
    public class MLAgentsGrasslandsMultiplayerLevelManager : GrasslandsMultiplayerLevelManager
    {
        public enum GameMode {Single, Team}

        public GameMode currentGameMode;

        private int[] GetTeamDeaths()
        {
            var teamDeaths = new[] { 0, 0 };
            foreach (var character in Instance.Players)
            {
                if (character.Dead)
                {
                    var index = character.TeamId - 1;
                    teamDeaths[index]++;
                }
            }

            return teamDeaths;
        }

        protected override bool GameOverCondition()
        {
            if (currentGameMode == GameMode.Single)
            {
                return base.GameOverCondition();
            }

            var teamDeaths = GetTeamDeaths();

            Debug.Log(teamDeaths[0] + "\t" + teamDeaths[1]);
            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }

        public bool IsWinner(TopDownAgent agent)
        {
            if (currentGameMode == GameMode.Single)
            {
                var winner = agent.AgentInput.PlayerId == WinnerID;
                return winner;
            }
            else
            {
                var teamDeaths = GetTeamDeaths();
                var character = agent.GetComponent<Character>();
                var winningId = teamDeaths[0] > teamDeaths[1] ? 1 : 2;
                var winner = character.TeamId == winningId;
                return winner;
            }
        }

        protected override IEnumerator GameOver()
        {
            Debug.Log("GameOver");
            var agents = FindObjectsOfType<TopDownAgent>();
            foreach (var agent in agents)
            {
                agent.Done();
            }
            foreach (var agent in agents)
            {
                var winner = IsWinner(agent);
                var reward = winner ? 1f : -0.25f;
                agent.SetReward(reward);
            }

            var enumerator = base.GameOver();
            Restart();
            return enumerator;
        }

        protected virtual void Restart()
        { 
            var characters = FindObjectsOfType<Character>();
            foreach (var character in characters)
            {
                Destroy(character.gameObject);
            }
            Initialization();
            SpawnMultipleCharacters();
        }

        public override void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            base.OnMMEvent(engineEvent);
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.MlCuriculum:
                    var dur = Academy.Instance.FloatProperties.GetPropertyWithDefault("game_duration",
                                                                                        8);
                    GameDuration = (int) dur;
                    UpdateCountdown();
                    break;
            }
        }

        public int GetPoints(string agentId)
        {
            var points = Points.SingleOrDefault(point => point.PlayerID == WinnerID);
            return points.Points;
        }
    }
}
