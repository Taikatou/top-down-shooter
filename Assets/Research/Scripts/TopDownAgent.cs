using System.Collections.Generic;
using MLAgents;
using UnityEngine;

namespace Research.Scripts
{
    public enum Directions { None, Left, Right, Up, Down }

    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        private Vector2 _transform;

        public override void OnActionReceived(float[] vectorAction)
        {
            // Extrinsic Penalty
            // AddReward(-1f / 3000f);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[0]);

            inputManager.SetAiPrimaryMovement(primaryDirection);
        }

        public override float[] Heuristic()
        {
            Debug.Log("Heuristic");
            var output = new []
            {
                (float)directionsKeyMapper.PrimaryDirections,  
                (float)directionsKeyMapper.SecondaryDirections
            };
            Debug.Log(output);

            return output;
        }

        public void Success()
        {
            AddReward(1);
            EndEpisode();
        }

        public override void OnEpisodeBegin()
        {
            
        }
    }
}
