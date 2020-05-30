using System;
using AgentInput;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Research.Common.Utils;
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

        public Rigidbody2D groundRb;

        public bool enableCurriculum = true;

        public float punishValue = -0.0005f;
        
        public override void Initialize()
        {
            trainingSettings = new TrainingSettings
            {
                ShootEnabled = true, 
                PunishTime = false, 
                SecondaryAbilityEnabled = false, 
                SecondaryInputEnabled = true
            };
            observationSettings = new ObservationSettings
            {
                ObserveHealth = true,
                ObserveSpriteRenderer = true,
                ObserveWeaponTrace = false,
                ObserveInput = true
            };
        }

        private float Increment
        {
            get
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
        }

        private void PunishMovement()
        {
            if (trainingSettings.PunishTime)
            {
                AddReward(punishValue);
            }
        }

        public float debugDirection;
        
        public override void OnActionReceived(float[] vectorAction)
        {
            var counter = 0;
            // Extrinsic Penalty
            var action = vectorAction[counter++];
            var primaryDirection = directionsKeyMapper.GetVectorDirection(action);
            debugDirection = action;
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
                var secondaryXInput = AgentUtils.GetDecision(vectorAction[counter++], Increment);
                var secondaryYInput = AgentUtils.GetDecision(vectorAction[counter++], Increment);
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
            if (enableCurriculum)
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
