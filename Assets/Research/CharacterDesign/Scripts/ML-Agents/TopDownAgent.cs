using System;
using System.Collections.Generic;
using MLAgents;
using MLAgents.Sensors;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
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

        public SpriteRenderer spriteRenderer;

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
            actionsOut[index++] = (int)directionsKeyMapper.PrimaryDirections;
            
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
                actionsOut[index] = secondaryShootButtonInput;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // sensor.AddObservation(_behaviorParameters.TeamId);
            sensor.AddObservation(HealthInput);

            var id = SpriteId.Instance.GetId(spriteRenderer);
            
            foreach(var result in id)
            {
                sensor.AddObservation(result);   
            }
        }
    }

    public class SpriteId
    {
        private Dictionary<string, int> _mapper;
        private int _counter;

        private static SpriteId _instance;
        public static SpriteId Instance => _instance ?? (_instance = new SpriteId());

        private SpriteId()
        {
            _mapper = new Dictionary<string, int>();
            var animIds = new []{
                "Damage", 
                "DashParticle", 
                "Dead", 
                "Falling", 
                "Idle", 
                "Run", 
                "SwordIdle",
                "SwordSlash1",
                "SwordSlash2",
                "SwordSlash3",
                "KoalaDamage"
            };
                
            AddIds(animIds);
        }

        public void AddIds(string[] ids)
        {
            foreach (var id in ids)
            {
                _mapper[id] = _counter++;
            }
        }

        private int[] GetId(string name)
        {
            try
            {
                var split = name.Split('_');
                var indexAvailable = split.Length == 3;
                var index = indexAvailable? int.Parse(split[2]) : 0;
                var animIndex = indexAvailable ? 1 : 0;
                var anim =  _mapper[split[animIndex]];
                var results = new int [] {anim, index};
                return results;
            }
            catch
            {
                Debug.Log(name);
                throw;
            }
        }

        public int[] GetId(SpriteRenderer spriteRenderer)
        {
            return GetId(spriteRenderer.sprite.name);
        }
    }
}
