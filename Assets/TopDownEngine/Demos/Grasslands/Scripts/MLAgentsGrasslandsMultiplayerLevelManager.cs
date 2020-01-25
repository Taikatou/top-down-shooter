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
        protected override IEnumerator GameOver()
        {
            Debug.Log("GameOver");
            var agents = FindObjectsOfType<TopDownAgent>();
            foreach(var agent in agents)
            {
                var winner = agent.AgentInput.PlayerId == WinnerID;
                var reward = winner ? 1f : -0.25f;
                agent.SetReward(reward);
            }

            foreach (var agent in agents)
            {
                agent.Done();
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

        protected override void SpawnMultipleCharacters()
        {
            base.SpawnMultipleCharacters();
            var agents = FindObjectsOfType<TopDownAgent>();
            foreach (var agent in agents)
            {
            }
        }
    }
}
