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
            if (healthComponent.CurrentHealth > 0)
            {
                var agentRb = GetComponent<Rigidbody2D>();
                var position = agentRb.transform.position;
                var newPosition = new Vector2(position.x - currentPosition.x, position.y - currentPosition.y);
                var positionArray = ISenseMethods.ToArray(newPosition);
                
                var behaviorName = GetComponent<BehaviorParameters>();
                var character = GetComponent<Character>();

                var playerStats = new List<float>
                {
                    healthComponent.CurrentHealth,
                    behaviorName.GetObservations(),
                    NameChecker.Index(character.teamTag)
                };

                return positionArray.Concat(playerStats).ToArray();
            }

            return new[] {0.0f, 0.0f, 0.0f};
        }
    }
}
