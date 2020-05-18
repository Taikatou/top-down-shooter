using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Cinemachine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Automatically grabs a Cinemachine camera group and assigns LevelManager's players on load and makes a Cinemachine Virtual Camera follow that target
    /// </summary>
    public class MultiplayerCameraGroupTarget : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<TopDownEngineEvent>
    {
        /// the virtual camera that will follow the group target
        public CinemachineVirtualCamera TargetCamera;

        protected CinemachineTargetGroup _targetGroup;

        private Character [] Players => FindObjectsOfType<Character>();

        /// <summary>
        /// On Awake we grab our target group component
        /// </summary>
        protected virtual void Awake()
        {
            _targetGroup = gameObject.GetComponent<CinemachineTargetGroup>();
        }

        /// <summary>
        /// On load, we bind the characters to the target group and have the virtual cam follow that target group
        /// </summary>
        /// <param name="gameEvent"></param>
        public virtual void OnMMEvent(MMGameEvent gameEvent)
        {
            if (gameEvent.EventName == "Load")
            {
                if (_targetGroup == null)
                {
                    return;
                }

                var i = 0;
                _targetGroup.m_Targets = new CinemachineTargetGroup.Target[Players.Length];

                foreach (var character in Players)
                {
                    CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
                    target.weight = 1;
                    target.radius = 0;
                    target.target = character.transform;

                    _targetGroup.m_Targets[i] = target;
                    i++;
                }

                TargetCamera.Follow = transform;
            }
        }

        public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
        {
            if (tdEvent.EventType == TopDownEngineEventTypes.PlayerDeath)
            {
                int i = 0;
                foreach (Character character in Players)
                {
                    if (character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                    {
                        _targetGroup.m_Targets[i].weight = 0f;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Starts listening for game events
        /// </summary>
        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<TopDownEngineEvent>();
        }

        /// <summary>
        /// Stops listening for game events
        /// </summary>
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<TopDownEngineEvent>();
        }
    }
}