﻿using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{	
	/// <summary>
	/// This class allows for a 3D character to grab its current weapon's handles, and look wherever it's aiming.
	/// There's a bit of setup involved. You need to have a CharacterHandleWeapon component on your character, it needs an animator with IKPass active (this is set in the Layers tab of the animator)
	/// the animator's avatar MUST be set as humanoid
	/// And you need to put that script on the same gameobject as the animator (otherwise it won't work). 
	/// Finally, you need to set left and right handles (or only one of these) on your weapon(s). 
	/// </summary>
	public class WeaponIK : MonoBehaviour
    {
        [Header("Bindings")]
        public Transform LeftHandTarget = null;
        public Transform RightHandTarget = null;

        [Header("Attachments")]
        public bool AttachLeftHand = true;
        public bool AttachRightHand = true;
        
        protected Animator _animator;

		protected virtual void Start()
		{
			_animator = GetComponent<Animator> ();
		}
        
		/// <summary>
		/// During the animator's IK pass, tries to attach the avatar's hands to the weapon
		/// </summary>
		protected virtual void OnAnimatorIK(int layerIndex)
		{
			if (_animator == null)
			{
				return;
			}

			//if the IK is active, set the position and rotation directly to the goal. 

            if (AttachLeftHand)
            {
                if (LeftHandTarget != null)
                {
                    AttachHandToHandle(AvatarIKGoal.LeftHand, LeftHandTarget);

                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(LeftHandTarget.position);
                }
                else
                {
                    DetachHandFromHandle(AvatarIKGoal.LeftHand);
                }
            }
			
            if (AttachRightHand)
            {
                if (RightHandTarget != null)
                {
                    AttachHandToHandle(AvatarIKGoal.RightHand, RightHandTarget);
                }
                else
                {
                    DetachHandFromHandle(AvatarIKGoal.RightHand);
                }
            }
			

		}  

        /// <summary>
        /// Attaches the hands to the handles
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handle"></param>
		protected virtual void AttachHandToHandle(AvatarIKGoal hand, Transform handle)
		{
			_animator.SetIKPositionWeight(hand,1);
			_animator.SetIKRotationWeight(hand,1);  
			_animator.SetIKPosition(hand,handle.position);
			_animator.SetIKRotation(hand,handle.rotation);
		}

		/// <summary>
		/// Detachs the hand from handle, if the IK is not active, set the position and rotation of the hand and head back to the original position
		/// </summary>
		/// <param name="hand">Hand.</param>
		protected virtual void DetachHandFromHandle(AvatarIKGoal hand)
		{
			_animator.SetIKPositionWeight(hand,0);
			_animator.SetIKRotationWeight(hand,0); 
			_animator.SetLookAtWeight(0);
		}

		/// <summary>
		/// Binds the character hands to the handles targets
		/// </summary>
		/// <param name="leftHand">Left hand.</param>
		/// <param name="rightHand">Right hand.</param>
		public virtual void SetHandles(Transform leftHand, Transform rightHand)
		{
			LeftHandTarget = leftHand;
			RightHandTarget = rightHand;
		}
	}
}
