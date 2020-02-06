using System;
using System.Collections;
using System.Linq;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
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
                var winner = IsWinner(agent);
                if (winner == GameEnding.Win)
                {
                    agent.AddReward(1.0f);
                }
                else if (winner == GameEnding.Loss)
                {
                    agent.AddReward(-0.25f);
                }

                agent.Done();
            }

            AppendResult(agents, GameEnding.Win);
            AppendResult(agents, GameEnding.Loss);
            AppendResult(agents, GameEnding.Draw);


            var enumerator = base.GameOver();
            Restart();
            return enumerator;
        }

        private void AppendResult(TopDownAgent[] agents, GameEnding condition)
        {
            var logger = FindObjectOfType<DataLogger>();
            var winners = Array.FindAll(agents, agent => IsWinner(agent) == condition);
            Debug.Log(winners.Length);
            if (winners.Length > 0)
            {
                Array.Sort(winners, (x, y) => string.CompareOrdinal(x.BehaviourName, y.BehaviourName));
                var winnersName = string.Join("", winners.Select((z => z.BehaviourName)));

                
                logger.AddResultTeam(winnersName, condition);

                foreach (var agent in winners)
                {
                    logger.AddResultAgent(agent.BehaviourName, condition);
                }
            }
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

        public float damageScale = 1;

        public override void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            base.OnMMEvent(engineEvent);
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.MlCuriculum:
                    Debug.Log("Reset Event");
                    var dur = Academy.Instance.FloatProperties.GetPropertyWithDefault("game_duration",
                                                                                        160);
                    var scale = Academy.Instance.FloatProperties.GetPropertyWithDefault("damage_scale",
                        2);
                    damageScale = scale;
                    GameDuration = (int) dur;
                    // StartCoroutine(GameOver());
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
