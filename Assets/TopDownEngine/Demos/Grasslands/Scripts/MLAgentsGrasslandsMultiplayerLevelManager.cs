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
        protected override IEnumerator GameOver()
        {
            var agents = FindObjectsOfType<TopDownAgent>();
            var winner = agents.SingleOrDefault(player => player.AgentInput.PlayerId == WinnerID);
            if (winner)
            {
                winner?.AddReward(1.0f);
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
    }
}
