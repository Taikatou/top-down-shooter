﻿using MLAgents.Policies;
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
            var agent = picker.GetComponent<BehaviorParameters>();
            var teamId = agent.TeamId;
            
            
            if (agent)
            {
                foreach (var player in levelManager.Players)
                {
                    var playerAgent = player.GetComponent<TopDownAgent>();
                    var playerBehaviour = player.GetComponent<BehaviorParameters>();
                    if (playerAgent)
                    {
                        var reward = playerBehaviour.TeamId == teamId ? 1 : -1;
                        playerAgent.AddReward(reward);
                        playerAgent.EndEpisode();
                    }
                }
            }
        }
    }
}