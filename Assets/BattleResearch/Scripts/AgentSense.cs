﻿using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class AgentSense : MonoBehaviour, ISense
    {
        public Dictionary<string, float> GetObservations()
        {
            var senses = GetObservationsList();
            return senses;
        }
        
        public Dictionary<string, float>  GetObservationsList()
        {
            var healthComponent = GetComponent<Health>();

            var behaviorName = GetComponent<BehaviorParameters>();
            var character = GetComponent<Character>();

            var playerStats = new Dictionary<string, float>()
            {
                { "Current Health", healthComponent.CurrentHealth / healthComponent.MaximumHealth },
                { "Behavior Name", behaviorName.GetObservations() },
                { "Team ID", character.TeamId }
            };

            var senses = playerStats;

            return senses;
        }

        public Dictionary<string, float>  GetObservationsOtherAgent(Vector2 currentPosition)
        {
            var agentRb = GetComponent<Rigidbody2D>();
            var position = agentRb.transform.position;
            var positionX = GetPositionScaled(position.x - currentPosition.x);
            var positionY = GetPositionScaled(position.y - currentPosition.y);

            var playerStats = GetObservationsList();

            playerStats.Add("PositionX", positionX);
            playerStats.Add("PositionY", positionY);

            return playerStats;
        }

        public float GetPositionScaled(float position)
        {
            var positionNewX = Mathf.Clamp(position, -25, 25);
            positionNewX = positionNewX /= 25;
            return positionNewX;
        }
    }
}
