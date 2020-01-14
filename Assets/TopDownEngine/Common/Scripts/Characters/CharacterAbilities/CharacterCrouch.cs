using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This ability allows the character to "crouch" when pressing the crouch button, which resizes the collider
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Crouch")]
    public class CharacterCrouch : CharacterAbility 
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles crouch and crawl behaviours. Here you can determine the crouch speed, and whether or not the collider should resize when crouched (to crawl into tunnels for example). If it should, please setup its new size here."; }
        
		[Header("Crawl")]
		/// if this is set to false, the character won't be able to crawl, just to crouch
		public bool CrawlAuthorized = true;
		/// the speed of the character when it's crouching
		public float CrawlSpeed = 4f;

		[Space(10)]	
		[Header("Crouching")]
		/// if this is true, the collider will be resized when crouched
		public bool ResizeColliderWhenCrouched = false;
		/// the size to apply to the collider when crouched (if ResizeColliderWhenCrouched is true, otherwise this will be ignored)
		public float CrouchedColliderHeight = 1.25f;
		/// if this is true, the character is crouched and has an obstacle over its head that prevents it from getting back up again

		[Space(10)]	
		[Header("Offset")]
        /// a list of objects to offset when crouching
		public List<GameObject> ObjectsToOffset;
        /// the offset to apply to objects when crouching
		public Vector3 OffsetCrouch;
        /// the offset to apply to objects when crouching AND moving
		public Vector3 OffsetCrawl;
        /// the speed at which to offset objects
		public float OffsetSpeed = 5f;

        /// whether or not the character is in a tunnel right now and can't get up
        [ReadOnly]
		public bool InATunnel;
        /// whether or not the character is in forced crouch mode (being forced to crouch by another object
        [ReadOnly]
        public bool ForcedCrouch = false;

        protected List<Vector3> _objectsToOffsetOriginalPositions;

        protected const string _crouchingAnimationParameterName = "Crouching";
        protected const string _crawlingAnimationParameterName = "Crawling";
        protected int _crouchingAnimationParameter;
        protected int _crawlingAnimationParameter;

        /// <summary>
        /// On Start(), we set our tunnel flag to false
        /// </summary>
        protected override void Initialization()
		{
			base.Initialization();
			InATunnel = false;

			// we store our objects to offset's initial positions
			if (ObjectsToOffset.Count > 0)
			{
				_objectsToOffsetOriginalPositions = new List<Vector3> ();
				foreach(GameObject go in ObjectsToOffset)
				{
                    if (go != null)
                    {
                        _objectsToOffsetOriginalPositions.Add(go.transform.localPosition);
                    }					
				}
			}
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			DetermineState ();
			CheckExitCrouch();
			OffsetObjects ();
		}

		/// <summary>
		/// At the start of the ability's cycle, we check if we're pressing down. If yes, we call Crouch()
		/// </summary>
		protected override void HandleInput()
		{			
			base.HandleInput ();

			// Crouch Detection : if the player is pressing "down" and if the character is grounded and the crouch action is enabled
			if (_inputManager.CrouchButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)		
			{
				Crouch();
			}

            if (ForcedCrouch && (_movement.CurrentState != CharacterStates.MovementStates.Crouching) && (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
            {
                Crouch();
            }
		}

        /// <summary>
        /// Forces the character to crouch
        /// </summary>
        public virtual void StartForcedCrouch()
        {
            ForcedCrouch = true;
        }

        /// <summary>
        /// Stops any forced crouch state the character could be in
        /// </summary>
        public virtual void StopForcedCrouch()
        {
            ForcedCrouch = false;
        }

		/// <summary>
		/// If we're pressing down, we check if we can crouch or crawl, and change states accordingly
		/// </summary>
		protected virtual void Crouch()
		{
			if (!AbilityPermitted// if the ability is not permitted
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)// or if we're not in our normal stance
			    || (!_controller.Grounded))// or if we're grounded
				// we do nothing and exit
			{
				return;
			}				

			// if this is the first time we're here, we trigger our sounds
			if ((_movement.CurrentState != CharacterStates.MovementStates.Crouching) && (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
			{
				// we play the crouch start sound 
				PlayAbilityStartSfx();
				PlayAbilityUsedSfx();
                PlayAbilityStartFeedbacks();
            }

			// we set the character's state to Crouching and if it's also moving we set it to Crawling
			_movement.ChangeState(CharacterStates.MovementStates.Crouching);
			if ( (Mathf.Abs(_horizontalInput) > 0) && (CrawlAuthorized) )
			{
				_movement.ChangeState(CharacterStates.MovementStates.Crawling);
			}

			// we resize our collider to match the new shape of our character (it's usually smaller when crouched)
			if (ResizeColliderWhenCrouched)
			{
				_controller.ResizeCollider(CrouchedColliderHeight);		
			}

			// we change our character's speed
			if (_characterMovement != null)
			{
				_characterMovement.MovementSpeed = CrawlSpeed;
			}

			// we prevent movement if we can't crawl
			if (!CrawlAuthorized)
			{
				_characterMovement.MovementSpeed = 0f;
			}
		}

		protected virtual void OffsetObjects ()
		{
			// we move all the objects we want to move
			if (ObjectsToOffset.Count > 0)
			{
				for (int i = 0; i < ObjectsToOffset.Count; i++)
				{
					Vector3 newOffset = Vector3.zero;
					if (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
					{
						newOffset = OffsetCrouch;
					}
					if (_movement.CurrentState == CharacterStates.MovementStates.Crawling)
					{
						newOffset = OffsetCrawl;
					}
                    if (ObjectsToOffset[i] != null)
                    {
                        ObjectsToOffset[i].transform.localPosition = Vector3.Lerp(ObjectsToOffset[i].transform.localPosition, _objectsToOffsetOriginalPositions[i] + newOffset, Time.deltaTime * OffsetSpeed);
                    }					
				}
			}
		}

		/// <summary>
		/// Runs every frame to check if we should switch from crouching to crawling or the other way around
		/// </summary>
		protected virtual void DetermineState()
		{
			if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching) || (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
			{
				if ( (_controller.CurrentMovement.magnitude > 0) && (CrawlAuthorized) )
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crawling);
				}
				else
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crouching);
				}
			}
		}

		/// <summary>
		/// Every frame, we check to see if we should exit the Crouching (or Crawling) state
		/// </summary>
		protected virtual void CheckExitCrouch()
		{				
			// if we're currently grounded
			if ( (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
			{	
				// but we're not pressing down anymore, or we're not grounded anymore
				if ( (!_controller.Grounded)
					|| ((_inputManager.CrouchButton.State.CurrentState == MMInput.ButtonStates.Off) && (!ForcedCrouch)))
				{
					// we cast a raycast above to see if we have room enough to go back to normal size
					InATunnel = !_controller.CanGoBackToOriginalSize();

					// if the character is not in a tunnel, we can go back to normal size
					if (!InATunnel)
					{
						// we return to normal walking speed
						if (_characterMovement != null)
						{
							_characterMovement.ResetSpeed ();
						}

						// we play our exit sound
						StopAbilityUsedSfx();
						PlayAbilityStopSfx();

                        // exit feedbacks
                        StopStartFeedbacks();
                        PlayAbilityStopFeedbacks();

                        // we go back to Idle state and reset our collider's size
                        _movement.ChangeState(CharacterStates.MovementStates.Idle);
						_controller.ResetColliderSize();
					}
				}
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter (_crouchingAnimationParameterName, AnimatorControllerParameterType.Bool, out _crouchingAnimationParameter);
			RegisterAnimatorParameter (_crawlingAnimationParameterName, AnimatorControllerParameterType.Bool, out _crawlingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator,_crawlingAnimationParameter,(_movement.CurrentState == CharacterStates.MovementStates.Crawling), _character._animatorParameters);
		}
	}
}
