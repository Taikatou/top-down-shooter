using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this ability to a character, and it'll be able to rotate to face the movement's direction or the weapon's rotation
    /// </summary>
    [MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Orientation 3D")]
    public class CharacterOrientation3D : CharacterAbility
    {
        /// the possible rotation modes
		public enum RotationModes { None, MovementDirection, WeaponDirection, Both }
        /// the possible rotation speeds
		public enum RotationSpeeds { Instant, Smooth, SmoothAbsolute }
               
        [Header("Rotation Mode")]
        /// whether the character should face movement direction, weapon direction, or both, or none
		public RotationModes RotationMode = RotationModes.None;
        /// if this is false, no rotation will occur
        public bool CharacterRotationAuthorized = true;

        [Header("Movement Direction")]
        /// If this is true, we'll rotate our model towards the direction
        public bool ShouldRotateToFaceMovementDirection = true;
        /// the current rotation mode
        [MMCondition("ShouldRotateToFaceMovementDirection", true)]
        public RotationSpeeds MovementRotationSpeed = RotationSpeeds.Instant;
        /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
        [MMCondition("ShouldRotateToFaceMovementDirection", true)]
        public GameObject MovementRotatingModel;
        /// the speed at which to rotate towards direction (smooth and absolute only)
        [MMCondition("ShouldRotateToFaceMovementDirection", true)]
        public float RotateToFaceMovementDirectionSpeed = 10f;
        /// the threshold after which we start rotating (absolute mode only)
        [MMCondition("ShouldRotateToFaceMovementDirection", true)]
        public float AbsoluteThresholdMovement = 0.5f;
        [MMReadOnly]
        /// the direction of the model
        public Vector3 ModelDirection;
        [MMReadOnly]
        /// the direction of the model in angle values
        public Vector3 ModelAngles;

        [Header("Weapon Direction")]
        /// If this is true, we'll rotate our model towards the weapon's direction
        public bool ShouldRotateToFaceWeaponDirection = true;
        /// the current rotation mode
        [MMCondition("ShouldRotateToFaceWeaponDirection", true)]
        public RotationSpeeds WeaponRotationSpeed = RotationSpeeds.Instant;
        /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
        [MMCondition("ShouldRotateToFaceWeaponDirection", true)]
        public GameObject WeaponRotatingModel;
        /// the speed at which to rotate towards direction (smooth and absolute only)
        [MMCondition("ShouldRotateToFaceWeaponDirection", true)]
        public float RotateToFaceWeaponDirectionSpeed = 10f;
        /// the threshold after which we start rotating (absolute mode only)
        [MMCondition("ShouldRotateToFaceWeaponDirection", true)]
        public float AbsoluteThresholdWeapon = 0.5f;

        [Header("Animation")]
        /// the speed at which the instant rotation animation parameter float resets to 0
        public float RotationSpeedResetSpeed = 2f;

        [Header("Forced Rotation")]
        /// whether the character is being applied a forced rotation
        public bool ForcedRotation = false;
        /// the forced rotation applied by an external script
        [MMCondition("ForcedRotation", true)]
        public Vector3 ForcedRotationDirection;

        protected CharacterHandleWeapon _characterHandleWeapon;
        protected CharacterRun _characterRun;
        protected Vector3 _lastRegisteredVelocity;
        protected Vector3 _rotationDirection;
        protected Vector3 _lastMovement = Vector3.zero;
        protected Vector3 _lastAim = Vector3.zero;

        protected Vector3 _relativeSpeed;
        protected Vector3 _remappedSpeed = Vector3.zero;
        protected Vector3 _relativeSpeedNormalized;
        protected bool _secondaryMovementTriggered = false;

        protected Quaternion _tmpRotation;
        protected Quaternion _newMovementQuaternion;
        protected Quaternion _newWeaponQuaternion;
        protected bool _shouldRotateTowardsWeapon;
        protected float _rotationSpeed;
        protected float _modelAnglesYLastFrame;
        protected Vector3 _currentDirection;

        protected const string _relativeForwardSpeedAnimationParameterName = "RelativeForwardSpeed";
        protected const string _relativeLateralSpeedAnimationParameterName = "RelativeLateralSpeed";
        protected const string _remappedForwardSpeedAnimationParameterName = "RemappedForwardSpeed";
        protected const string _remappedLateralSpeedAnimationParameterName = "RemappedLateralSpeed";
        protected const string _relativeForwardSpeedNormalizedAnimationParameterName = "RelativeForwardSpeedNormalized";
        protected const string _relativeLateralSpeedNormalizedAnimationParameterName = "RelativeLateralSpeedNormalized";
        protected const string _rotationSpeeddAnimationParameterName = "YRotationSpeed";
        protected int _relativeForwardSpeedAnimationParameter;
        protected int _relativeLateralSpeedAnimationParameter;
        protected int _remappedForwardSpeedAnimationParameter;
        protected int _remappedLateralSpeedAnimationParameter;
        protected int _relativeForwardSpeedNormalizedAnimationParameter;
        protected int _relativeLateralSpeedNormalizedAnimationParameter;
        protected int _rotationSpeeddAnimationParameter;


        /// <summary>
        /// On init we grab our model if necessary
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            if (MovementRotatingModel == null)
            {
                MovementRotatingModel = _model;
            }
            _characterRun = this.gameObject.GetComponent<CharacterRun>();
            _characterHandleWeapon = this.gameObject.GetComponent<CharacterHandleWeapon>();
            if (WeaponRotatingModel == null)
            {
                WeaponRotatingModel = _model;
            }
        }

        /// <summary>
        /// Every frame we rotate towards the direction
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            if (GameManager.Instance.Paused)
            {
                return;
            }
            if (CharacterRotationAuthorized)
            {
                RotateToFaceMovementDirection();
                RotateToFaceWeaponDirection();
                RotateModel();
            }            
        }


        protected virtual void LateUpdate()
        {
            ComputeRelativeSpeeds();
        }


        /// <summary>
        /// Rotates the player model to face the current direction
        /// </summary>
        protected virtual void RotateToFaceMovementDirection()
        {
            // if we're not supposed to face our direction, we do nothing and exit
            if (!ShouldRotateToFaceMovementDirection) { return; }
            if ((RotationMode != RotationModes.MovementDirection) && (RotationMode != RotationModes.Both)) { return; }

            _currentDirection = ForcedRotation ? ForcedRotationDirection : _controller.CurrentDirection;
            
            // if the rotation mode is instant, we simply rotate to face our direction
            if (MovementRotationSpeed == RotationSpeeds.Instant)
            {
                if (_currentDirection != Vector3.zero)
                {
                    _newMovementQuaternion = Quaternion.LookRotation(_currentDirection);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction
            if (MovementRotationSpeed == RotationSpeeds.Smooth)
            {
                if (_currentDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_currentDirection);
                    _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
                }                
            }

            // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
            if (MovementRotationSpeed == RotationSpeeds.SmoothAbsolute)
            {
                if (_currentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
                {
                    _lastMovement = _currentDirection;
                }
                if (_lastMovement != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_lastMovement);
                    _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
                }
            }

            ModelDirection = MovementRotatingModel.transform.forward.normalized;
            ModelAngles = MovementRotatingModel.transform.eulerAngles;
        }

        /// <summary>
        /// Rotates the character so it faces the weapon's direction
        /// </summary>
		protected virtual void RotateToFaceWeaponDirection()
        {
            _newWeaponQuaternion = Quaternion.identity;
            _shouldRotateTowardsWeapon = false;

            // if we're not supposed to face our direction, we do nothing and exit
            if (!ShouldRotateToFaceWeaponDirection) { return; }
            if ((RotationMode != RotationModes.WeaponDirection) && (RotationMode != RotationModes.Both)) { return; }
            if (_characterHandleWeapon == null) { return; }
            if (_characterHandleWeapon.WeaponAimComponent == null) { return; }

            _shouldRotateTowardsWeapon = true;

            _rotationDirection = _characterHandleWeapon.WeaponAimComponent.CurrentAim.normalized;
                        
            MMDebug.DebugDrawArrow(this.transform.position, _rotationDirection, Color.red);

            // if the rotation mode is instant, we simply rotate to face our direction
            if (WeaponRotationSpeed == RotationSpeeds.Instant)
            {
                if (_rotationDirection != Vector3.zero)
                {                    
                    _newWeaponQuaternion = Quaternion.LookRotation(_rotationDirection);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction
            if (WeaponRotationSpeed == RotationSpeeds.Smooth)
            {
                if (_rotationDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_rotationDirection);
                    _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
            if (WeaponRotationSpeed == RotationSpeeds.SmoothAbsolute)
            {
                if (_rotationDirection.normalized.magnitude >= AbsoluteThresholdWeapon)
                {
                    _lastMovement = _rotationDirection;
                }
                if (_lastMovement != Vector3.zero)
                {
                    _tmpRotation = Quaternion.LookRotation(_lastMovement);
                    _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
                }
            }
        }

        /// <summary>
        /// Rotates models if needed
        /// </summary>
        protected virtual void RotateModel()
        {
            MovementRotatingModel.transform.rotation = _newMovementQuaternion;
            
            if (_shouldRotateTowardsWeapon)
            {
                WeaponRotatingModel.transform.rotation = _newWeaponQuaternion;
            }
        }

        /// <summary>
        /// Computes the relative speeds
        /// </summary>
        protected virtual void ComputeRelativeSpeeds()
        {
            // relative speed
            if (_characterHandleWeapon == null)
            {
                _relativeSpeed = MovementRotatingModel.transform.InverseTransformVector(_controller.CurrentMovement);
            }
            else
            {
                _relativeSpeed = WeaponRotatingModel.transform.InverseTransformVector(_controller.CurrentMovement);
            }

            // remapped speed
            if (_characterRun != null)
            {
                _remappedSpeed.x = MMMaths.Remap(_relativeSpeed.x, 0f, _characterRun.RunSpeed, 0f, 1f);
                _remappedSpeed.y = MMMaths.Remap(_relativeSpeed.y, 0f, _characterRun.RunSpeed, 0f, 1f);
                _remappedSpeed.z = MMMaths.Remap(_relativeSpeed.z, 0f, _characterRun.RunSpeed, 0f, 1f);
            }            

            // relative speed normalized
            _relativeSpeedNormalized = _relativeSpeed.normalized;

            // RotationSpeed
            if (Mathf.Abs(_modelAnglesYLastFrame - ModelAngles.y) > 1f)
            {
                _rotationSpeed = Mathf.Abs(_modelAnglesYLastFrame - ModelAngles.y);
            }
            else
            {
                _rotationSpeed -= Time.time * RotationSpeedResetSpeed;
            }
            if (_rotationSpeed <= 0f)
            {
                _rotationSpeed = 0f;
            }

            _modelAnglesYLastFrame = ModelAngles.y;
        }

        /// <summary>
        /// Forces the character's model to face in the specified direction
        /// </summary>
        /// <param name="direction"></param>
        public virtual void Face(Character.FacingDirections direction)
        {
            switch(direction)
            {
                case Character.FacingDirections.East:
                    _newMovementQuaternion = Quaternion.LookRotation(Vector3.right);
                    break;
                case Character.FacingDirections.North:
                    _newMovementQuaternion = Quaternion.LookRotation(Vector3.forward);
                    break;
                case Character.FacingDirections.South:
                    _newMovementQuaternion = Quaternion.LookRotation(Vector3.back);
                    break;
                case Character.FacingDirections.West:
                    _newMovementQuaternion = Quaternion.LookRotation(Vector3.left);
                    break;
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_rotationSpeeddAnimationParameterName, AnimatorControllerParameterType.Float, out _rotationSpeeddAnimationParameter);
            RegisterAnimatorParameter(_relativeForwardSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeForwardSpeedAnimationParameter);
            RegisterAnimatorParameter(_relativeLateralSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeLateralSpeedAnimationParameter);
            RegisterAnimatorParameter(_remappedForwardSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _remappedForwardSpeedAnimationParameter);
            RegisterAnimatorParameter(_remappedLateralSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _remappedLateralSpeedAnimationParameter);
            RegisterAnimatorParameter(_relativeForwardSpeedNormalizedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeForwardSpeedNormalizedAnimationParameter);
            RegisterAnimatorParameter(_relativeLateralSpeedNormalizedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeLateralSpeedNormalizedAnimationParameter);
        }

        /// <summary>
        /// Sends the current speed and the current value of the Walking state to the animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _rotationSpeeddAnimationParameter, _rotationSpeed, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeForwardSpeedAnimationParameter, _relativeSpeed.z, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeLateralSpeedAnimationParameter, _relativeSpeed.x, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _remappedForwardSpeedAnimationParameter, _remappedSpeed.z, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _remappedLateralSpeedAnimationParameter, _remappedSpeed.x, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeForwardSpeedNormalizedAnimationParameter, _relativeSpeedNormalized.z, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeLateralSpeedNormalizedAnimationParameter, _relativeSpeedNormalized.x, _character._animatorParameters);
        }
    }
}