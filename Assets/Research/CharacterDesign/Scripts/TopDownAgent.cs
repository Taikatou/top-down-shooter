using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Characters;
using Research.Common;
using Research.Common.Weapons;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.CharacterDesign.Scripts
{
    public class TopDownAgent : Agent
    {
        public ObservationSettings observationSettings;

        public TrainingSettings trainingSettings;

        public float punishValue = -0.0005f;

        public MlCharacter character;

        private void PunishMovement()
        {
            if (trainingSettings.punishTime)
            {
                AddReward(punishValue);
            }
        }
        
        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            character.UpdateFrame();
            PunishMovement();
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
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
            base.CollectObservations(sensor);
            if (observationSettings.observeWeaponTrace)
            {
                var weaponTrace = GetComponentInChildren<WeaponRayTrace>();
                var traceOutput = weaponTrace ? weaponTrace.GetRay() : 0.0f;
                sensor.AddObservation(traceOutput);
            }

            if (observationSettings.observeCloseToWall)
            {
                var closeWall = GetComponentInParent<MlCloseToWall>();
                var closeWallOutput = closeWall && closeWall.CanShoot;
                sensor.AddObservation(closeWallOutput);
            }

            if (observationSettings.observeHealth)
            {
                var health = GetComponentInParent<Health>();

                sensor.AddObservation(health.CurrentHealth); 
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {

        }
    }
}
