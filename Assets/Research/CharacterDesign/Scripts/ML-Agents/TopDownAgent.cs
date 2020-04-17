using System;
using MLAgents;
using MLAgents.Sensors;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.AgentInput;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public enum Directions { None, Left, Right, Up, Down }

    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        public SecondaryDirectionsInput secondaryDirectionsInput;

        private Health _health;

        public bool secondaryInputEnabled;
        public bool shootEnabled;
        public bool secondaryAbilityEnabled;

        private float HealthInput => _health.CurrentHealth / _health.MaximumHealth;

        public override void Initialize()
        {
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
            var counter = 0;
            // Extrinsic Penalty
            // AddReward(-1f / 3000f);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[counter++]);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if (shootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetShootButton(shootButtonDown);
            }

            if (secondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = GetDecision(vectorAction[counter++]);
                var secondaryYInput = GetDecision(vectorAction[counter++]);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                inputManager.SetAiSecondaryMovement(secondary);
            }

            if (secondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
        }

        public override void Heuristic(float[] actionsOut)
        {
            var index = 0;
            if (shootEnabled)
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                var shootButtonInput = Convert.ToSingle(shootButtonState);
                actionsOut[index++] = shootButtonInput;
            }

            if (secondaryInputEnabled)
            {
                var secondaryDirections = secondaryDirectionsInput.SecondaryDirection;
                actionsOut[index++] = secondaryDirections.x;
                actionsOut[index++] = secondaryDirections.y;
            }
            
            if (secondaryAbilityEnabled)
            {
                var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);
                actionsOut[index++] = secondaryShootButtonInput;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // sensor.AddObservation(_behaviorParameters.TeamId);
            sensor.AddObservation(HealthInput);
        }
    }
}
