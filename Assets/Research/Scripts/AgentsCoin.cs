using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts
{
    public class AgentsCoin : PickableItem
    {
        public GrasslandsMultiplayerLevelManager levelManager;
        // Update is called once per frame
        protected override void Pick(GameObject picker)
        {
            var agent = picker.GetComponent<TopDownAgent>();
            if (agent)
            {
                agent.Success();
                foreach (var player in levelManager.Players)
                {
                    var playerAgent = player.GetComponent<TopDownAgent>();
                    if (playerAgent)
                    {
                        playerAgent.EndEpisode();
                    }
                }
            }
        }
    }
}
