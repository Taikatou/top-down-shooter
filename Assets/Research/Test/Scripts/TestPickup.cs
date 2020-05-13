using System.Collections.Generic;
using Research.Common.MapSensor;
using Research.Common.MapSensor.GridSpaceEntity;
using Research.LevelDesign.NuclearThrone.Scripts;
using Unity.MLAgents.Policies;
using UnityEngine;

namespace Research.Test.Scripts
{
    public class TestPickup : EntityMapPosition
    {
        public GameObject playersParent;

        public override GridSpace GetType(int teamId) => GridSpace.Coin;

        private TestGridAgent [] Players => playersParent.GetComponentsInChildren<TestGridAgent>();

        public void OnTriggerEnter2D(Collider2D agentCollider)
        {
            var agent = agentCollider.GetComponent<BehaviorParameters>();

            if (agent)
            {
                var teamId = agent.TeamId;
                foreach (var player in Players)
                {
                    var playerBehaviour = player.GetComponent<BehaviorParameters>();
                    if (player)
                    {
                        var reward = playerBehaviour.TeamId == teamId ? 1 : -1;
                        player.AddReward(reward);
                        player.EndEpisode();
                        Debug.Log("End Episode");
                    }
                }
                
                Debug.Log("Reward Found");
            }
        }

        public void ResetPosition(Transform position)
        {
            transform.position = position.position;
        }
    }
}
