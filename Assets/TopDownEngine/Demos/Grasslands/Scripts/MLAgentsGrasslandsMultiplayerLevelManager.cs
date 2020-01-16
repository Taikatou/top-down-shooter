using System.Collections;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngine.Demos.Grasslands.Scripts
{
    public class MLAgentsGrasslandsMultiplayerLevelManager : GrasslandsMultiplayerLevelManager
    {
        protected override IEnumerator GameOver()
        {
            var enumerator = base.GameOver();
            Restart();
            return enumerator;
        }

        protected virtual void Restart()
        {
            Debug.Log("Restart");
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RespawnStarted, null);
            var characters = FindObjectsOfType<Character>();
            foreach (var character in characters)
            {
                Destroy(character);
            }
            Initialization();
        }
    }
}
