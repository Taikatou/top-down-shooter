using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	[AddComponentMenu("TopDown Engine/Environment/Teleporter")]
	/// <summary>
	/// Add this script to a trigger collider2D to teleport objects from that object to its destination
	/// </summary>
	[SelectionBase]
	public class Teleporter : ButtonActivated 
	{
		[Header("Teleporter")]
		/// the teleporter's destination
		public Teleporter Destination;
		/// if true, this won't teleport non player characters
		public bool OnlyAffectsPlayer=true;

		[Header("Teleporter Camera")]
		/// if this is true, the camera will teleport instantly to the teleporter's destination when activated
		public bool TeleportCamera = false;
		/// if this is true, a fade to black will occur when teleporting
		public bool FadeToBlack = false;
        /// the curve to use to fade to black
        [MMCondition("FadeToBlack")]
        public MMTween.MMTweenCurve FaderCurve;
        /// the ID of the fader to use 
        [MMCondition("FadeToBlack")]
        public int FaderID = 0;
        /// the duration (in seconds) of the fade to black  
        [MMCondition("FadeToBlack")]
        public float FadeDuration = 0f;
        /// the duration (in seconds) between the fade in and the fade out
        [MMCondition("FadeToBlack", true)]
        public float BetweenFadeDuration = 0.5f;
        /// whether or not the character should be frozen during the fade
        [MMCondition("FadeToBlack", true)]
        public bool FreezeDuringFade = true;

        protected Character _player;
	    protected List<Transform> _ignoreList;

	    /// <summary>
	    /// On start we initialize our ignore list
	    /// </summary>
	    protected virtual void Start()
		{		
			_ignoreList = new List<Transform>();
        }

	    /// <summary>
	    /// Triggered when something enters the teleporter
	    /// </summary>
	    /// <param name="collider">Collider.</param>
		protected override void TriggerEnter(GameObject collider)
		{
            // if the object that collides with the teleporter is on its ignore list, we do nothing and exit.
			if (_ignoreList.Contains(collider.transform))
            {
                return;
			}			

			if (collider.GetComponent<Character>()!=null)
			{                
                _player = collider.GetComponent<Character>();
			}

            // if the teleporter is supposed to only affect the player (well, TopDownControllers), we do nothing and exit
            if (OnlyAffectsPlayer || !AutoActivation)
			{
				base.TriggerEnter(collider);
            }
			else
			{
                Teleport(collider);
			}
		}

		/// <summary>
		/// If we're button activated and if the button is pressed, we teleport
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			if (_player != null)
			{
				base.TriggerButtonAction ();
				Teleport(_player.gameObject);
			}
			ActivateZone ();
		}

		/// <summary>
		/// Teleports whatever enters the portal to a new destination
		/// </summary>
		protected virtual void Teleport(GameObject collider)
		{
			// if the teleporter has a destination, we move the colliding object to that destination
			if (Destination!=null)
			{
                StartCoroutine(TeleportSequence(collider));
			}
		}

        protected virtual IEnumerator TeleportSequence(GameObject collider)
        {

            BeforeFadeIn(collider);

            if (FadeToBlack)
            {
                MMFadeInEvent.Trigger(FadeDuration, FaderCurve, FaderID, false, collider.transform.position);
                FadeInComplete(collider);
                yield return new WaitForSeconds(FadeDuration);
            }
            else
            {
                FadeInComplete(collider);
            }


            if (FadeToBlack)
            {
                yield return new WaitForSeconds(BetweenFadeDuration);
            }

            AfterFadePause(collider);

            if (FadeToBlack)
            {
                MMFadeOutEvent.Trigger(FadeDuration, FaderCurve, FaderID, false, collider.transform.position);
            }

            AfterFadeOut(collider);
        }

        /// <summary>
        /// Describes the events happening before the initial fade in
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void BeforeFadeIn(GameObject collider)
        {
            ActivateZone();
            if (TeleportCamera)
            {
                MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing, collider.MMGetComponentNoAlloc<Character>());
            }
        }

        /// <summary>
        /// Describes the events happening once the initial fade in is complete
        /// </summary>
        protected virtual void FadeInComplete(GameObject collider)
        {
            collider.transform.position = Destination.transform.position;
            _ignoreList.Remove(collider.transform);
            Destination.AddToIgnoreList(collider.transform);
            if (FreezeDuringFade)
            {
                collider.MMGetComponentNoAlloc<Character>().ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
            }            
        }

        /// <summary>
        /// Describes the events happening after the pause between the fade in and the fade out
        /// </summary>
        protected virtual void AfterFadePause(GameObject collider)
        {
            if (TeleportCamera)
            {
                MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing, collider.MMGetComponentNoAlloc<Character>());

            }
        }

        /// <summary>
        /// Describes the events happening after the fade out is complete, so at the end of the teleport sequence
        /// </summary>
        protected virtual void AfterFadeOut(GameObject collider)
        {
            if (FreezeDuringFade)
            {
                collider.MMGetComponentNoAlloc<Character>().ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            }            
        }

        /// <summary>
        /// When something exits the teleporter, if it's on the ignore list, we remove it from it, so it'll be considered next time it enters.
        /// </summary>
        /// <param name="collider">Collider.</param>
        public override void TriggerExitAction(GameObject collider)
        {
            base.TriggerExitAction(collider);
            if (_ignoreList.Contains(collider.transform))
			{
				_ignoreList.Remove(collider.transform);
			}
		}
		
		/// <summary>
		/// Adds an object to the ignore list, which will prevent that object to be moved by the teleporter while it's in that list
		/// </summary>
		/// <param name="objectToIgnore">Object to ignore.</param>
		public virtual void AddToIgnoreList(Transform objectToIgnore)
		{
			_ignoreList.Add(objectToIgnore);
		}
    }
}