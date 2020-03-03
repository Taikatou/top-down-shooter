using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class AgentSense : MonoBehaviour, ISense
    {
        public bool IncludePosition = false;
        public Dictionary<string, float> GetObservations()
        {
            var senses = GetObservationsList();
            return senses;
        }

        public string SenseName => "AgentSense";

        public Dictionary<string, float>  GetObservationsList()
        {
            var healthComponent = GetComponent<Health>();

            var behaviorName = GetComponent<BehaviorParameters>();
            var character = GetComponent<Character>();

            var playerStats = new Dictionary<string, float>()
            {
                { "Current Health", healthComponent.CurrentHealth / healthComponent.MaximumHealth },
                { "Behavior Name", NameChecker.Index(behaviorName.Name)},
                { "Team ID", character.TeamId }
            };

            var senses = playerStats;

            return senses;
        }

        public Dictionary<string, float>  GetObservationsOtherAgent(Vector2 currentPosition)
        {
            var playerStats = GetObservationsList();
            if (IncludePosition)
            {
                var agentRb = GetComponent<Rigidbody2D>();
                var position = agentRb.transform.position;
                var positionX = GetPositionScaled(position.x - currentPosition.x);
                var positionY = GetPositionScaled(position.y - currentPosition.y);

                playerStats.Add("PositionX", positionX);
                playerStats.Add("PositionY", positionY);
            }
            

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
