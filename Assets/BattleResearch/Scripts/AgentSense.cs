using System.Collections.Generic;
using System.Linq;
using MLAgents;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace BattleResearch.Scripts
{
    public class AgentSense : MonoBehaviour, ISense
    {
        public float[] GetObservations()
        {
            return GetObservations(new Vector2());
        }
        
        public float[] GetObservations(Vector2 currentPosition)
        {
            var healthComponent = GetComponent<Health>();
            var agentRb = GetComponent<Rigidbody2D>();
            var position = agentRb.transform.position;

            var positionX = GetPositionScaled(position.x - currentPosition.x);
            var positionY = GetPositionScaled(position.y - currentPosition.y);

            var behaviorName = GetComponent<BehaviorParameters>();
            var character = GetComponent<Character>();

            var playerStats = new List<float>
            {
                positionX,
                positionY,
                healthComponent.CurrentHealth,
                behaviorName.GetObservations(),
                character.TeamId
            };

            var senses = playerStats.ToArray();

            return senses;
        }

        public float GetPositionScaled(float position)
        {
            var positionNewX = Mathf.Clamp(position, -25, 25);
            positionNewX = positionNewX /= 25;
            return positionNewX;
        }
    }
}
