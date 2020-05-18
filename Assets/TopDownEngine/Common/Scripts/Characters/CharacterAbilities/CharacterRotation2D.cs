using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this ability to a character and it'll rotate or flip to face the direction of movement or the weapon's, or both, or none
    /// Only add this ability to a 2D character
    /// </summary>
    [MMHiddenProperties("AbilityStartFeedbacks", "AbilityStopFeedbacks")]
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Rotation 2D")]
    public class CharacterRotation2D : CharacterAbility
    {
        /// the possible rotation modes
		public enum RotationModes { None, MovementDirection, WeaponDirection, Both }
        /// the possible rotation speeds
		public enum RotationSpeeds { Instant, Smooth, SmoothAbsolute }

        [Header("Rotation Mode")]
        /// whether the character should face movement direction, weapon direction, or both, or none
		public RotationModes RotationMode = RotationModes.None;

        [Header("Movement Direction")]
        /// If this is true, we'll rotate our model towards the direction
        public bool ShouldRotateToFaceMovementDirection = true;
        /// the current rotation mode
        public RotationSpeeds MovementRotationSpeed = RotationSpeeds.Instant;
        /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
        public GameObject MovementRotatingModel;
        /// the speed at which to rotate towards direction (smooth and absolute only)
        public float RotateToFaceMovementDirectionSpeed = 10f;
        /// the threshold after which we start rotating (absolute mode only)
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
        public RotationSpeeds WeaponRotationSpeed = RotationSpeeds.Instant;
        /// the object we want to rotate towards direction. If left empty, we'll use the Character's model
        public GameObject WeaponRotatingModel;
        /// the speed at which to rotate towards direction (smooth and absolute only)
        public float RotateToFaceWeaponDirectionSpeed = 10f;
        /// the threshold after which we start rotating (absolute mode only)
        public float AbsoluteThresholdWeapon = 0.5f;

        protected CharacterHandleWeapon _characterHandleWeapon;
        protected Vector3 _lastRegisteredVelocity;
        protected Vector3 _rotationDirection;
        protected Vector3 _lastMovement = Vector3.zero;
        protected Vector3 _lastAim = Vector3.zero;

        protected Vector3 _relativeSpeed;
        protected Vector3 _relativeSpeedNormalized;
        protected bool _secondaryMovementTriggered = false;

        protected Quaternion _tmpRotation;
        protected Quaternion _newMovementQuaternion;
        protected Quaternion _newWeaponQuaternion;
        protected bool _shouldRotateTowardsWeapon;


        protected const string _relativeForwardSpeedAnimationParameterName = "RelativeForwardSpeed";
        protected const string _relativeLateralSpeedAnimationParameterName = "RelativeLateralSpeed";
        protected const string _relativeForwardSpeedNormalizedAnimationParameterName = "RelativeForwardSpeedNormalized";
        protected const string _relativeLateralSpeedNormalizedAnimationParameterName = "RelativeLateralSpeedNormalized";
        protected int _relativeForwardSpeedAnimationParameter;
        protected int _relativeLateralSpeedAnimationParameter;
        protected int _relativeForwardSpeedNormalizedAnimationParameter;
        protected int _relativeLateralSpeedNormalizedAnimationParameter;

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

            _characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
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
            RotateToFaceMovementDirection();
            RotateToFaceWeaponDirection();
            RotateModel();
        }


        protected virtual void FixedUpdate()
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

            // if the rotation mode is instant, we simply rotate to face our direction

            float angle = Mathf.Atan2(_controller.CurrentDirection.y, _controller.CurrentDirection.x) * Mathf.Rad2Deg;

            if (MovementRotationSpeed == RotationSpeeds.Instant)
            {
                if (_controller.CurrentDirection != Vector3.zero)
                {
                    _newMovementQuaternion = Quaternion.Euler(angle * Vector3.forward);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction
            if (MovementRotationSpeed == RotationSpeeds.Smooth)
            {
                if (_controller.CurrentDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.Euler(angle * Vector3.forward);
                    _newMovementQuaternion = Quaternion.Slerp(MovementRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceMovementDirectionSpeed);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
            if (MovementRotationSpeed == RotationSpeeds.SmoothAbsolute)
            {
                if (_controller.CurrentDirection.normalized.magnitude >= AbsoluteThresholdMovement)
                {
                    _lastMovement = _controller.CurrentDirection;
                }
                if (_lastMovement != Vector3.zero)
                {
                    float lastAngle = Mathf.Atan2(_lastMovement.y, _lastMovement.x) * Mathf.Rad2Deg;
                    _tmpRotation = Quaternion.Euler(lastAngle * Vector3.forward);

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
            
            float angle = Mathf.Atan2(_rotationDirection.y, _rotationDirection.x) * Mathf.Rad2Deg;

            // if the rotation mode is instant, we simply rotate to face our direction
            if (WeaponRotationSpeed == RotationSpeeds.Instant)
            {
                if (_rotationDirection != Vector3.zero)
                {
                    _newWeaponQuaternion = Quaternion.Euler(angle * Vector3.forward);
                }
            }

            // if the rotation mode is smooth, we lerp towards our direction
            if (WeaponRotationSpeed == RotationSpeeds.Smooth)
            {
                if (_rotationDirection != Vector3.zero)
                {
                    _tmpRotation = Quaternion.Euler(angle * Vector3.forward);
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
                    float lastAngle = Mathf.Atan2(_lastMovement.y, _lastMovement.x) * Mathf.Rad2Deg;
                    _tmpRotation = Quaternion.Euler(lastAngle * Vector3.forward);
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

        protected Vector3 _positionLastFrame;
        protected Vector3 _newSpeed;

        /// <summary>
        /// Computes the relative speeds
        /// </summary>
        protected virtual void ComputeRelativeSpeeds()
        {
            _newSpeed = (this.transform.position - _positionLastFrame) / Time.deltaTime;

            if (_characterHandleWeapon == null)
            {
                _relativeSpeed = MovementRotatingModel.transform.InverseTransformVector(_newSpeed);
            }
            else
            {
                _relativeSpeed = WeaponRotatingModel.transform.InverseTransformVector(_newSpeed);
            }
            _relativeSpeedNormalized = _relativeSpeed.normalized;
            _positionLastFrame = this.transform.position;
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_relativeForwardSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeForwardSpeedAnimationParameter);
            RegisterAnimatorParameter(_relativeLateralSpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeLateralSpeedAnimationParameter);
            RegisterAnimatorParameter(_relativeForwardSpeedNormalizedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeForwardSpeedNormalizedAnimationParameter);
            RegisterAnimatorParameter(_relativeLateralSpeedNormalizedAnimationParameterName, AnimatorControllerParameterType.Float, out _relativeLateralSpeedNormalizedAnimationParameter);
        }

        /// <summary>
        /// Sends the current speed and the current value of the Walking state to the animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeForwardSpeedAnimationParameter, _relativeSpeed.z, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeLateralSpeedAnimationParameter, _relativeSpeed.x, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeForwardSpeedNormalizedAnimationParameter, _relativeSpeedNormalized.z, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _relativeLateralSpeedNormalizedAnimationParameter, _relativeSpeedNormalized.x, _character._animatorParameters);
        }
    }
}
