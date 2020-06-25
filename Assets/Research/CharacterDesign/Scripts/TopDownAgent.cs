using System;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.AgentInput;
using Research.CharacterDesign.Scripts.Characters;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Research.Common.SpriteSensor;
using Research.Common.Utils;
using Research.Common.Weapons;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;

namespace Research.CharacterDesign.Scripts
{
    public abstract class TopDownAgent : Agent
    {
        public TopDownInputManager inputManager;

        public DirectionsKeyMapper directionsKeyMapper;

        public SecondaryDirectionsInput secondaryDirectionsInput;

        public ObservationSettings observationSettings;

        public TrainingSettings trainingSettings;

        public float punishValue = -0.0005f;

        public bool enableHeuristic;

        public Health [] agentHealths;

        public MlCharacter character;
        
        protected abstract void ObserveWeapon(VectorSensor sensor);

        protected abstract void OnActionReceivedImp(float[] vectorAction);
        
        protected abstract void HeuristicImp(float[] actionsOut);

        private void PunishMovement()
        {
            if (trainingSettings.punishTime)
            {
                AddReward(punishValue);
            }
        }
        
        public override void OnActionReceived(float[] vectorAction)
        {
            OnActionReceivedImp(vectorAction);
            character.UpdateFrame();
            PunishMovement();
        }

        public override void Heuristic(float[] actionsOut)
        {
            if (enableHeuristic)
            {
                HeuristicImp(actionsOut);
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
                
                // Debug.Log(traceOutput);
            }

            if (observationSettings.observeCloseToWall)
            {
                var closeWall = GetComponentInParent<MlCloseToWall>();
                var closeWallOutput = closeWall && closeWall.CanShoot;
                sensor.AddObservation(closeWallOutput);
            }

            if (observationSettings.observeInput)
            {
                ObserveWeapon(sensor);
            }

            if (observationSettings.observeHealth)
            {
                foreach (var health in agentHealths)
                {
                    sensor.AddObservation(health.CurrentHealth);
                }
            }
        }
    }
}
