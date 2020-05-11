using System;
using AgentInput;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        public SecondaryDirectionsInput secondaryDirectionsInput;

        public SpriteRenderer spriteRenderer;

        public float gunSpeed = 0.01f;
        
        public AimControl aimControl = AimControl.ThirtyTwoWay;
        
        public ObservationSettings observationSettings;

        public TrainingSettings trainingSettings;

        private Rigidbody2D _mAgentRb;

        public Rigidbody2D groundRb;

        public bool enableCuriculuum = true;

        public float punishValue = -0.0005f;
        
        public override void Initialize()
        {
            _mAgentRb = GetComponent<Rigidbody2D>();
            trainingSettings = new TrainingSettings
            {
                ShootEnabled = true, 
                PunishTime = true, 
                SecondaryAbilityEnabled = false, 
                SecondaryInputEnabled = true
            };
            observationSettings = new ObservationSettings
            {
                ObserveHealth = true,
                ObserveSpriteRenderer = true,
                ObserveWeaponTrace = true,
                ObserveInput = true
            };
        }
        
        private float GetDecision(float input)
        {
            switch (Mathf.FloorToInt(input))
            {
                case 1:
                    // Left or Down
                    return -1;
                case 2:
                    // Right or Up
                    return 1;
                case 3:
                    // Right or Up
                    return -GetIncrement();
                case 4:
                    // Right or Up
                    return GetIncrement();
                case 5:
                    // Right or Up
                    return -2 * GetIncrement();
                case 6:
                    // Right or Up
                    return 2 * GetIncrement();
            }
            return 0;
        }

        private float GetIncrement()
        {
            switch (aimControl)
            {
                case AimControl.SixTeenWay:
                    return 0.5f;
                case AimControl.ThirtyTwoWay:
                    return 0.33f;
            }
            return 1;
        }

        private void PunishMovement()
        {
            if (trainingSettings.PunishTime)
            {
                AddReward(punishValue);
            }
        }
        
        public override void OnActionReceived(float[] vectorAction)
        {
            var counter = 0;
            // Extrinsic Penalty
            var primaryDirection = directionsKeyMapper.GetVectorDirection(vectorAction[counter++]);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if (trainingSettings.ShootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetShootButton(shootButtonDown);
            }

            if (trainingSettings.SecondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = GetDecision(vectorAction[counter++]);
                var secondaryYInput = GetDecision(vectorAction[counter++]);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                switch (aimControl)
                {
                    case AimControl.Addition:
                        inputManager.MoveAiSecondaryMovement(secondary, gunSpeed);
                        break;
                    case AimControl.SixTeenWay:
                        inputManager.SetAiSecondaryMovement(secondary);
                        break;
                }
            }

            if (trainingSettings.SecondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }

            PunishMovement();
        }

        public override void Heuristic(float[] actionsOut)
        {
            var index = 0;
            actionsOut[index++] = (int) directionsKeyMapper.PrimaryDirections;
            
            if (trainingSettings.ShootEnabled)
            {
                var shootButtonState = Input.GetKey(KeyCode.X);
                var shootButtonInput = Convert.ToSingle(shootButtonState);
                actionsOut[index++] = shootButtonInput;
            }

            if (trainingSettings.SecondaryInputEnabled)
            {
                var secondaryDirections = secondaryDirectionsInput.SecondaryDirection;
                actionsOut[index++] = secondaryDirections.x;
                actionsOut[index++] = secondaryDirections.y;
            }
            
            if (trainingSettings.SecondaryAbilityEnabled)
            {
                var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);
                Debug.Log(index);
                actionsOut[index] = secondaryShootButtonInput;
            }
        }

        public override void OnEpisodeBegin()
        {
            if (enableCuriculuum)
            {
                var mResetParams = Academy.Instance.EnvironmentParameters;
                var levelDesign = mResetParams.GetWithDefault("agent_level_setup", 0);
                var agentQueue = GetComponentInParent<AgentQueue>();
                if (agentQueue)
                {
                    var curriculum = (LevelCurriculum) levelDesign;
                    Debug.Log("Current Curriculum:" + curriculum);
                    agentQueue.currentCurriculum = curriculum;
                }   
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (observationSettings.ObserveSpriteRenderer)
            {
                var id = SpriteId.Instance.GetId(spriteRenderer);
            
                foreach(var result in id)
                {
                    sensor.AddObservation(result);   
                }   
            }
            if (observationSettings.ObserveWeaponTrace)
            {
                var weaponTrace = GetComponentInChildren<WeaponRayTrace>();
                var traceOutput = weaponTrace ? weaponTrace.GetRay() : 0.0f;
            
                sensor.AddObservation(traceOutput);
            }

            if (observationSettings.ObserveInput)
            {
                sensor.AddObservation(inputManager.PrimaryMovement);
                sensor.AddObservation(inputManager.SecondaryMovement);
            }
            
            // var agentPos = _mAgentRb.position - groundRb.position;
            // sensor.AddObservation(agentPos / 20f);
        }
    }
}
