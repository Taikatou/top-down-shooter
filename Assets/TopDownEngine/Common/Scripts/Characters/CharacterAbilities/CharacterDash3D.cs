 using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this ability to a 3D character and it'll be able to dash (cover the specified distance in the specified time)
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Dash 3D")]
    public class CharacterDash3D : CharacterAbility
    {
        /// the possible dash modes (fixed = always the same direction)
        public enum DashModes { Fixed, MainMovement, SecondaryMovement, MousePosition }
        /// the current dash mode
        public DashModes DashMode = DashModes.MainMovement;
        
        [Header("Dash")]
        /// the direction of the dash, relative to the character
        public Vector3 DashDirection = Vector3.forward;
        /// the distance to cover
        public float DashDistance = 10f;
        /// the duration of the dash
        public float DashDuration = 0.5f;
        /// the curve to apply to the dash's acceleration
        public AnimationCurve DashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [Header("Cooldown")]
        /// this ability's cooldown
        public MMCooldown Cooldown;
        
        /// the feedbacks to play when dashing
        public MMFeedbacks DashFeedback;

        protected bool _dashing;
        protected bool _dashStartedThisFrame;
        protected float _dashTimer;
        protected Vector3 _dashOrigin;
        protected Vector3 _dashDestination;
        protected Vector3 _newPosition;
        protected Vector3 _dashAngle = Vector3.zero;
        protected Vector3 _inputDirection;
        protected Plane _playerPlane;
        protected Camera _mainCamera;
        protected const string _dashingAnimationParameterName = "Dashing";
        protected const string _dashStartedAnimationParameterName = "DashStarted";
        protected int _dashingAnimationParameter;
        protected int _dashStartedAnimationParameter;

        /// <summary>
        /// On init we initialize our cooldown and feedback
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _playerPlane = new Plane(Vector3.up, Vector3.zero);
            _mainCamera = Camera.main;
            Cooldown.Initialization();
            DashFeedback?.Initialization(this.gameObject);
        }

        /// <summary>
        /// Watches for input and starts a dash if needed
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.Jumping))
            {
                return;
            }
            if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                DashStart();
            }
        }

        /// <summary>
        /// Starts a dash
        /// </summary>
        public virtual void DashStart()
        {
            if (!Cooldown.Ready())
            {
                return;
            }
            Cooldown.Start();

            float angle  = 0f;
            _movement.ChangeState(CharacterStates.MovementStates.Dashing);
            _dashing = true;
            _dashTimer = 0f;
            _dashOrigin = this.transform.position;
            _controller.FreeMovement = false;
            DashFeedback?.PlayFeedbacks(this.transform.position);
            PlayAbilityStartFeedbacks();
            _dashStartedThisFrame = true;

            switch (DashMode)
            {
                case DashModes.MainMovement:
                    angle = Vector3.SignedAngle(this.transform.forward, _controller.CurrentDirection.normalized, Vector3.up);
                    _dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
                    _dashAngle.y = angle;
                    _dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);
                    break;

                case DashModes.Fixed:
                    _dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
                    break;

                case DashModes.SecondaryMovement:
                    _inputDirection = _character.LinkedInputManager.SecondaryMovement;
                    _inputDirection.z = _inputDirection.y;
                    _inputDirection.y = 0;

                    angle = Vector3.SignedAngle(this.transform.forward, _inputDirection.normalized, Vector3.up);
                    _dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
                    _dashAngle.y = angle;
                    _dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);

                    _controller.CurrentDirection = (_dashDestination - this.transform.position).normalized;
                    break;

                case DashModes.MousePosition:
                    Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
                    float distance;
                    if (_playerPlane.Raycast(ray, out distance))
                    {
                        _inputDirection = ray.GetPoint(distance);
                    }

                    angle = Vector3.SignedAngle(this.transform.forward, (_inputDirection - this.transform.position).normalized, Vector3.up);
                    _dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
                    _dashAngle.y = angle;
                    _dashDestination = MMMaths.RotatePointAroundPivot(_dashDestination, this.transform.position, _dashAngle);

                    _controller.CurrentDirection = (_dashDestination - this.transform.position).normalized;
                    break;
            }
        }

        /// <summary>
        /// Stops the dash
        /// </summary>
        protected virtual void DashStop()
        {
            Cooldown.Stop();
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _dashing = false;
            _controller.FreeMovement = true;
            DashFeedback?.StopFeedbacks();
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
        }

        /// <summary>
        /// On process ability, we move our character if we're currently dashing
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            Cooldown.Update();

            if (_dashing)
            {
                if (_dashTimer < DashDuration)
                {
                    _newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, DashCurve.Evaluate(_dashTimer / DashDuration));
                    _dashTimer += Time.deltaTime;
                    _controller.MovePosition(_newPosition);
                }
                else
                {
                    DashStop();                   
                }
            }
        }
        
        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
            RegisterAnimatorParameter(_dashStartedAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashStartedAnimationParameter);
        }

        /// <summary>
        /// At the end of each cycle, we send our Running status to the character's animator
        /// </summary>
        public override void UpdateAnimator()
        {
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashStartedAnimationParameter, _dashStartedThisFrame, _character._animatorParameters);

            _dashStartedThisFrame = false;
        }
    }
}