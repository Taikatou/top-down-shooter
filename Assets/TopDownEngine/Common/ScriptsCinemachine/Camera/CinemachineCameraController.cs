using UnityEngine;
using MoreMountains.Tools;
using Cinemachine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// A class that handles camera follow for Cinemachine powered cameras
    /// </summary>
    public class CinemachineCameraController : MonoBehaviour, MMEventListener<MMCameraEvent>, MMEventListener<TopDownEngineEvent>
    {
        /// True if the camera should follow the player
        public bool FollowsPlayer { get; set; }

        public bool FollowsAPlayer = true;
        public bool ConfineCameraToLevelBounds = true;
        [MMReadOnly]
        public Character TargetCharacter;

        protected CinemachineVirtualCamera _virtualCamera;
        protected CinemachineConfiner _confiner;

        /// <summary>
        /// On Awake we grab our components
        /// </summary>
        protected virtual void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _confiner = GetComponent<CinemachineConfiner>();
        }

        /// <summary>
        /// On Start we assign our bounding volume
        /// </summary>
        protected virtual void Start()
        {
            if ((_confiner != null) && ConfineCameraToLevelBounds)
            {
                _confiner.m_BoundingVolume = LevelManager.Instance.BoundsCollider;
            }
        }

        public virtual void SetTarget(Character character)
        {
            TargetCharacter = character;
        }

        /// <summary>
        /// Starts following the LevelManager's main player
        /// </summary>
        public virtual void StartFollowing()
        {
            if (!FollowsAPlayer) { return; }
            FollowsPlayer = true;
            _virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
        }

        /// <summary>
        /// Stops following any target
        /// </summary>
        public virtual void StopFollowing()
        {
            if (!FollowsAPlayer) { return; }
            FollowsPlayer = false;
            _virtualCamera.Follow = null;
        }

        public virtual void OnMMEvent(MMCameraEvent cameraEvent)
        {
            switch (cameraEvent.EventType)
            {
                case MMCameraEventTypes.SetTargetCharacter:
                    SetTarget(cameraEvent.TargetCharacter);
                    break;
                case MMCameraEventTypes.SetConfiner:                    
                    if (_confiner != null)
                    {
                        _confiner.m_BoundingVolume = cameraEvent.Bounds;
                    }
                    break;
                case MMCameraEventTypes.StartFollowing:
                    if (cameraEvent.TargetCharacter != null)
                    {
                        if (cameraEvent.TargetCharacter != TargetCharacter)
                        {
                            return;
                        }
                    }
                    StartFollowing();
                    break;

                case MMCameraEventTypes.StopFollowing:
                    if (cameraEvent.TargetCharacter != null)
                    {
                        if (cameraEvent.TargetCharacter != TargetCharacter)
                        {
                            return;
                        }
                    }
                    StopFollowing();
                    break;
            }
        }

        public virtual void OnMMEvent(TopDownEngineEvent topdownEngineEvent)
        {
            if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
            {
                SetTarget(LevelManager.Instance.Players[0]);
                StartFollowing();
            }

            if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwap)
            {
                SetTarget(LevelManager.Instance.Players[0]);
                StartFollowing();
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MMCameraEvent>();
            this.MMEventStartListening<TopDownEngineEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MMCameraEvent>();
            this.MMEventStopListening<TopDownEngineEvent>();
        }
    }
}
