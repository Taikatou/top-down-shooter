using System;
using MLAgents;
using MLAgents.Policies;
using MLAgents.Sensors;
using MoreMountains.TopDownEngine;
using Research.Scripts.AgentInput;
using UnityEngine;

namespace Research.Scripts
{
    public enum Directions { None, Left, Right, Up, Down }

    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        public SecondaryDirectionsInput secondaryDirectionsInput;

        private BehaviorParameters _behaviorParameters;

        private Health _health;

        private float HealthInput => _health.CurrentHealth / _health.MaximumHealth;

        public override void Initialize()
        {
            _behaviorParameters = GetComponent<BehaviorParameters>();
            _health = GetComponent<Health>();
        }
        
        private int GetDecision(float input)
        {
            switch (Mathf.FloorToInt(input))
            {
                case 1:
                    // Left or Down
                    return -1;
                case 2:
                    // Right or Up
                    return 1;
            }
            return 0;
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            // Extrinsic Penalty
            // AddReward(-1f / 3000f);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[0]);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            // Set secondary input as vector
            var secondaryXInput = GetDecision(vectorAction[1]);
            var secondaryYInput = GetDecision(vectorAction[2]);
            var secondary = new Vector2(secondaryXInput, secondaryYInput);
            
            inputManager.SetAiSecondaryMovement(secondary);
            
            // Shoot Button Input
            var shootButtonDown = Convert.ToBoolean(vectorAction[3]);
            inputManager.SetShootButton(shootButtonDown);
        }

        public override float[] Heuristic()
        {
            var shootButtonState = Input.GetKey(KeyCode.X);
            var shootButtonInput = Convert.ToSingle(shootButtonState);
            var output = new []
            {
                (float)directionsKeyMapper.PrimaryDirections,
                secondaryDirectionsInput.SecondaryDirection.x,
                secondaryDirectionsInput.SecondaryDirection.y,
                shootButtonInput
            };

            return output;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(_behaviorParameters.TeamId);
            sensor.AddObservation(HealthInput);
        }
    }
}
