using System;
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using BattleResearch.Scripts;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this ability to a character and it'll be able to dash in 2D, covering a certain distance in a certain duration
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Dash 2D")]
    public class CharacterDash2D : CharacterAbility, ISense
	{
        /// the possible dash modes (fixed : always the same direction)
        public enum DashModes { Fixed, MainMovement, SecondaryMovement, MousePosition }
        /// the dash mode to apply the dash in
        public DashModes DashMode = DashModes.MainMovement;

        [Header("Dash")]
        /// the dash direction
		public Vector3 DashDirection = Vector3.forward;
        /// the distance the dash should last for
		public float DashDistance = 6f;
        /// the duration of the dash, in seconds
		public float DashDuration = 0.2f;
        /// the animation curve to apply to the dash acceleration
		public AnimationCurve DashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        
        [Header("Cooldown")]
        /// this ability's cooldown
        public MMCooldown Cooldown;

        [Header("Feedback")]
        /// the feedbacks to play when dashing
        public MMFeedbacks DashFeedback;

        protected bool _dashing;
		protected float _dashTimer;
		protected Vector3 _dashOrigin;
		protected Vector3 _dashDestination;
		protected Vector3 _newPosition;
		protected Vector3 _dashAngle = Vector3.zero;
        protected Vector3 _inputPosition;
        protected Camera _mainCamera;
        protected const string _dashingAnimationParameterName = "Dashing";
        protected int _dashingAnimationParameter;

        /// <summary>
        /// On init, we stop our particles, and initialize our dash bar
        /// </summary>
		protected override void Initialization()
		{
			base.Initialization ();
            Cooldown.Initialization();

            _mainCamera = Camera.main;

            if (GUIManager.Instance != null && _character.CharacterType == Character.CharacterTypes.Player)
            {
                GUIManager.Instance.SetDashBar(true, _character.PlayerID);
                UpdateDashBar();
            }
        }

        /// <summary>
        /// Watches for dash inputs
        /// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }
            if (_inputManager.SecondaryShootButtonState == MMInput.ButtonStates.ButtonDown)
			{
				DashStart();
			}
		}

        /// <summary>
        /// Initiates the dash
        /// </summary>
		protected virtual void DashStart()
		{
			// Debug.Log("Dash");
            if (!Cooldown.Ready())
            {
                return;
            }

            Cooldown.Start();
            _movement.ChangeState(CharacterStates.MovementStates.Dashing);	
			_dashing = true;
			_dashTimer = 0f;
			_dashOrigin = this.transform.position;
            _controller.FreeMovement = false;
            DashFeedback?.PlayFeedbacks(this.transform.position);
            PlayAbilityStartFeedbacks();

            switch (DashMode)
            {
                case DashModes.MainMovement:
                    _dashDestination = this.transform.position + _controller.CurrentDirection.normalized * DashDistance;
                    break;

                case DashModes.Fixed:
                    _dashDestination = this.transform.position + DashDirection.normalized * DashDistance;
                    break;

                case DashModes.SecondaryMovement:
                    _dashDestination = this.transform.position + (Vector3)_character.LinkedInputManager.SecondaryMovement.normalized * DashDistance;
                    break;

                case DashModes.MousePosition:
                    _inputPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    _inputPosition.z = this.transform.position.z;
                    _dashDestination = this.transform.position + (_inputPosition - this.transform.position).normalized * DashDistance;
                    break;
            }            
        }

        /// <summary>
        /// Stops the dash
        /// </summary>
        protected virtual void DashStop()
        {
            DashFeedback?.StopFeedbacks(this.transform.position);

            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();

            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _dashing = false;
            _controller.FreeMovement = true;
        }

        /// <summary>
        /// On update, moves the character if needed
        /// </summary>
		public override void ProcessAbility()
        {
            base.ProcessAbility ();
            Cooldown.Update();
            UpdateDashBar();

            if (_dashing)
			{
				if (_dashTimer < DashDuration)
				{
					_newPosition = Vector3.Lerp (_dashOrigin, _dashDestination, DashCurve.Evaluate (_dashTimer/DashDuration));
					_dashTimer += Time.deltaTime;
					_controller.MovePosition (_newPosition);
				}
				else
				{
                    DashStop();
                }
			}
		}

        /// <summary>
		/// Updates the GUI jetpack bar.
		/// </summary>
		protected virtual void UpdateDashBar()
        {
            if ((GUIManager.Instance != null) && (_character.CharacterType == Character.CharacterTypes.Player))
            {
                GUIManager.Instance.UpdateDashBars(Cooldown.CurrentDurationLeft, 0f, Cooldown.ConsumptionDuration, _character.PlayerID);
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_dashingAnimationParameterName, AnimatorControllerParameterType.Bool, out _dashingAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _dashingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Dashing),_character._animatorParameters);
		}

        public Dictionary<string, float> GetObservations()
        {
            var obs = new Dictionary<string, float>
            {
	            { "Dasshing", Convert.ToSingle(_dashing) },
	            { "Dash Timer", _dashTimer },
	            { "Breather Cooldown", Cooldown.CurrentDurationLeft },
	            { "Cooldown State", (float) Cooldown.CooldownState }
            };
            return obs;
        }

        public string SenseName => "Dash2D";
	}
}
