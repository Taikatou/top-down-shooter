using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target. 
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionPathfinderToTarget3D")]
    [RequireComponent(typeof(CharacterMovement))]
    public class AIActionPathfinderToTarget3D : AIAction
    {
        protected CharacterMovement _characterMovement;
        protected CharacterPathfinder3D _characterPathfinder3D;

        /// <summary>
        /// On init we grab our CharacterMovement ability
        /// </summary>
        protected override void Initialization()
        {
            _characterMovement = this.gameObject.GetComponent<CharacterMovement>();
            _characterPathfinder3D = this.gameObject.GetComponent<CharacterPathfinder3D>();
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            Move();
        }

        /// <summary>
        /// Moves the character towards the target if needed
        /// </summary>
        protected virtual void Move()
        {
            if (_brain.Target == null)
            {
                _characterPathfinder3D.SetNewDestination(null);
                return;
            }
            else
            {
                _characterPathfinder3D.SetNewDestination(_brain.Target.transform);
            }
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement.SetHorizontalMovement(0f);
            _characterMovement.SetVerticalMovement(0f);
        }
    }
}
