using System;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Research.Common.SpriteSensor;
using Research.Common.Utils;
using Research.Common.Weapons;
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
        
        public AimControl aimControl = AimControl.EightWay;
        
        public ObservationSettings observationSettings;

        public TrainingSettings trainingSettings;

        public Rigidbody2D groundRb;

        public float punishValue = -0.0005f;

        public bool enableHeuristic;

        private void PunishMovement()
        {
            if (trainingSettings.punishTime)
            {
                AddReward(punishValue);
            }
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            var counter = 0;
            // Extrinsic Penalty
            var action = vectorAction[counter++];
            var primaryDirection = directionsKeyMapper.GetVectorDirection(action);
            inputManager.SetAiPrimaryMovement(primaryDirection);

            if (trainingSettings.shootEnabled)
            {
                // Shoot Button Input
                var shootButtonDown = Convert.ToBoolean(vectorAction[counter++]);
                inputManager.SetShootButton(shootButtonDown);
            }

            if (trainingSettings.secondaryInputEnabled)
            {
                // Set secondary input as vector
                var secondaryXInput = AgentUtils.GetDecision(vectorAction[counter++], aimControl);
                var secondaryYInput = AgentUtils.GetDecision(vectorAction[counter++], aimControl);
                var secondary = new Vector2(secondaryXInput, secondaryYInput);
                inputManager.SetAiSecondaryMovement(secondary);
            }

            if (trainingSettings.secondaryAbilityEnabled)
            {
                var secondaryShootButtonDown = Convert.ToBoolean(vectorAction[counter]);
                inputManager.SetSecondaryShootButton(secondaryShootButtonDown);
            }
            
            Debug.Log(vectorAction[2] + "\t" + vectorAction[3]);

            PunishMovement();
        }

        public override void Heuristic(float[] actionsOut)
        {
            if (enableHeuristic)
            {
                var index = 0;
                actionsOut[index++] = (int) directionsKeyMapper.PrimaryDirections;
                if (trainingSettings.shootEnabled)
                {
                    var shootButtonState = Input.GetKey(KeyCode.X);
                    var shootButtonInput = Convert.ToSingle(shootButtonState);
                    actionsOut[index++] = shootButtonInput;
                }

                if (trainingSettings.secondaryInputEnabled)
                {
                    var secondaryDirections = secondaryDirectionsInput.SecondaryDirection;
                    actionsOut[index++] = secondaryDirections.x;
                    actionsOut[index++] = secondaryDirections.y;
                }
            
                if (trainingSettings.secondaryAbilityEnabled)
                {
                    var secondaryShootButtonState = Input.GetKey(KeyCode.C);
                    var secondaryShootButtonInput = Convert.ToSingle(secondaryShootButtonState);
                    actionsOut[index] = secondaryShootButtonInput;
                }   
            }
        }

        public override void OnEpisodeBegin()
        {
            if (trainingSettings.enableCurriculum)
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
            if (observationSettings.observeWeaponTrace)
            {
                var weaponTrace = GetComponentInChildren<WeaponRayTrace>();
                var traceOutput = weaponTrace ? weaponTrace.GetRay() : 0.0f;
                sensor.AddObservation(traceOutput);
            }

            if (observationSettings.observeCloseToWall)
            {
                var closeWall = GetComponentInParent<MlCloseToWall>();
                var closeWallOutput = closeWall ? closeWall.CanShoot : false;
                sensor.AddObservation(closeWallOutput);
            }

            if (observationSettings.observeInput)
            {
                sensor.AddObservation(inputManager.PrimaryMovement);
                sensor.AddObservation(inputManager.SecondaryMovement);
            }
            
            // var agentPos = _mAgentRb.position - groundRb.position;
            // sensor.AddObservation(agentPos / 20f);
        }
    }
}
