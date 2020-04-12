using System;
using System.Collections.Generic;
using System.Linq;
using MLAgents;
using MLAgents.Policies;
using MLAgents.Sensors;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.TopDownEngineCustom;
using TopDownEngine.Common.Scripts.Characters.Core;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public enum Directions { None, Left, Right, Up, Down }

    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        public SecondaryDirectionsInput secondaryDirectionsInput;

        private BehaviorParameters _behaviorParameters;

        private Health _health;

        public bool secondaryInputEnabled = false;
        public bool shootEnabled;
        public bool secondaryAbilityEnabled;

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
            var counter = 0;
            // Extrinsic Penalty
            // AddReward(-1f / 3000f);
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[counter]);
            inputManager.SetAiPrimaryMovement(primaryDirection);
            counter++;

            if (shootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction[counter]);
                inputManager.SetShootButton(shootButtonDown);
                counter++;
            }

            if (secondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = GetDecision(vectorAction[counter]);
                counter++;
                var secondaryYInput = GetDecision(vectorAction[counter]);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                inputManager.SetAiSecondaryMovement(secondary);
                counter++;
            }

            if (secondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[counter]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
        }

        public override float[] Heuristic()
        {
            var output = new List<float>{ (float)directionsKeyMapper.PrimaryDirections };
            var shootButtonState = Input.GetKey(KeyCode.X);
            if (shootEnabled)
            {
                var shootButtonInput = Convert.ToSingle(shootButtonState);
                output.Add(shootButtonInput);
            }

            if (secondaryInputEnabled)
            {
                var secondaryDirections = secondaryDirectionsInput.SecondaryDirection;
                output.Add(secondaryDirections.x);
                output.Add(secondaryDirections.y);
            }
            
            if (secondaryAbilityEnabled)
            {
                var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);
                output.Add(secondaryShootButtonInput);
            }
            
            return output.ToArray();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // sensor.AddObservation(_behaviorParameters.TeamId);
            sensor.AddObservation(HealthInput);
        }
    }
}
