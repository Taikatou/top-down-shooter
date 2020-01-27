using System.Collections.Generic;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class AgentSense : MonoBehaviour, ISense
    {
        public float[] GetObservations()
        {
            var senses = GetObservationsList().ToArray();
            return senses;
        }
        
        public List<float> GetObservationsList()
        {
            var healthComponent = GetComponent<Health>();

            var behaviorName = GetComponent<BehaviorParameters>();
            var character = GetComponent<Character>();

            var playerStats = new List<float>
            {
                healthComponent.CurrentHealth / healthComponent.MaximumHealth,
                behaviorName.GetObservations(),
                character.TeamId
            };

            var senses = playerStats;

            return senses;
        }

        public float [] GetObservationsOtherAgent(Vector2 currentPosition)
        {
            var agentRb = GetComponent<Rigidbody2D>();
            var position = agentRb.transform.position;
            var positionX = GetPositionScaled(position.x - currentPosition.x);
            var positionY = GetPositionScaled(position.y - currentPosition.y);

            var playerStats = GetObservationsList();

            playerStats.Add(positionX);
            playerStats.Add(positionY);

            return playerStats.ToArray();
        }

        public float GetPositionScaled(float position)
        {
            var positionNewX = Mathf.Clamp(position, -25, 25);
            positionNewX = positionNewX /= 25;
            return positionNewX;
        }
    }
}
