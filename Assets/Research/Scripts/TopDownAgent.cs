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

        public override void OnActionReceived(float[] vectorAction)
        {
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[0]);
            inputManager.SetAiPrimaryMovement(primaryDirection);
        }

        public override float[] Heuristic()
        {
            var output = new []
            {
                (float)directionsKeyMapper.PrimaryDirections,  
                (float)directionsKeyMapper.SecondaryDirections
            };

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
