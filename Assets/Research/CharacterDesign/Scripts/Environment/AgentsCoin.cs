using MoreMountains.TopDownEngine;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.Test.Scripts;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Environment
{
    public class AgentsCoin : PickableItem
    {
        public GetEnvironmentMapPositions levelManager;
        // Update is called once per frame
        protected override void Pick(GameObject picker)
        {
            var agent = picker.GetComponent<BehaviorParameters>();
            var teamId = agent.TeamId;

            if (agent)
            {
                foreach (var player in levelManager.EntityMapPositions)
                {
                    var playerAgent = player.GetComponent<TestGridAgent>();
                    var playerBehaviour = player.GetComponent<BehaviorParameters>();
                    if (playerAgent)
                    {
                        var reward = playerBehaviour.TeamId == teamId ? 1 : -1;
                        playerAgent.AddReward(reward);
                        playerAgent.EndEpisode();
                    }
                }
                
                Debug.Log("Reward Found");

                levelManager.Restart();
            }
        }
    }
}
