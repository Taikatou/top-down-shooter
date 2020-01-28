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
            
            var gameOver = teamDeaths[0] == 2 || teamDeaths[1] == 2;
            return gameOver;
        }
        
        public enum GameEnding {Draw, Loss, Win}

        public GameEnding IsWinner(TopDownAgent agent)
        {
            if (currentGameMode == GameMode.Single)
            {
                var winner = agent.AgentInput.PlayerId == WinnerID;
                return winner? GameEnding.Win : GameEnding.Loss;
            }
            else
            {
                var teamDeaths = GetTeamDeaths();
                var character = agent.GetComponent<Character>();
                Debug.Log(teamDeaths[0] + "\t" + teamDeaths[1]);
                if ((teamDeaths[0] > 0 || teamDeaths[1] > 0) && teamDeaths[0] != teamDeaths[1])
                {
                    var winningId = teamDeaths[0] > teamDeaths[1] ? 1 : 2;
                    var winner = character.TeamId == winningId;
                    return winner? GameEnding.Win : GameEnding.Loss;
                }

                return GameEnding.Draw;
            }
        }

        protected override IEnumerator GameOver()
        {
            Debug.Log("GameOver");
            var agents = FindObjectsOfType<TopDownAgent>();
            foreach (var agent in agents)
            {
                agent.Done();
                
                var winner = IsWinner(agent);
                if (winner == GameEnding.Win)
                {
                    Debug.Log("Winner");
                    agent.SetReward(1.0f);
                }
                else if (winner == GameEnding.Loss)
                {
                    Debug.Log("Looser");
                    agent.SetReward(-0.25f);
                }
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
                    Debug.Log("Reset Event");
                    var dur = Academy.Instance.FloatProperties.GetPropertyWithDefault("game_duration",
                                                                                        160);
                    GameDuration = (int) dur;
                    StartCoroutine(GameOver());
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
