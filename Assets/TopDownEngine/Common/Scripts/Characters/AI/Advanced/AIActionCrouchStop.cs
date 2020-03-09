using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This action forces the character to stop crouching if it can
    /// </summary>
    public class AIActionCrouchStop : AIAction
    {
        protected CharacterCrouch _characterCrouch;
        protected Character _character;

        /// <summary>
        /// Grabs dependencies
        /// </summary>
        protected override void Initialization()
        {
            _character = this.gameObject.GetComponent<Character>();
            _characterCrouch = this.gameObject.GetComponent<CharacterCrouch>();
        }

        /// <summary>
        /// On PerformAction we stop crouching
        /// </summary>
        public override void PerformAction()
        {
            if ((_character == null) || (_characterCrouch == null))
            {
                return;
            }

            if ((_character.MovementState.CurrentState == CharacterStates.MovementStates.Crouching)
                || (_character.MovementState.CurrentState == CharacterStates.MovementStates.Crawling))
            {
                _characterCrouch.StopForcedCrouch();
            }
        }
    }
}
