using System.Collections;
using System.Linq;
using BattleResearch.Scripts;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngine.Demos.Grasslands.Scripts
{
    public class MLAgentsGrasslandsMultiplayerLevelManager : GrasslandsMultiplayerLevelManager
    {
        protected override IEnumerator GameOver()
        {
            var inputs = FindObjectsOfType<MlAgentInput>();
            var winner = inputs.SingleOrDefault(player => player.PlayerId == WinnerID);
            var agent = winner?.GetComponent<TopDownAgent>();
            if (agent)
            {
                agent.AddReward(1.0f);
            }
            var enumerator = base.GameOver();
            Restart();
            return enumerator;
        }

        protected virtual void Restart()
        {
            Debug.Log("Restart");
            var characters = FindObjectsOfType<Character>();
            foreach (var character in characters)
            {
                Destroy(character.gameObject);
            }
            Initialization();
            SpawnMultipleCharacters();
        }
    }
}
