using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this ability to a character, and it'll be able to control time when pressing the TimeControl button
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Time Control")]
    public class CharacterTimeControl : CharacterAbility
    {
        /// the time scale to switch to when the time control button gets pressed
        public float TimeScale = 0.5f;
        /// the duration for which to keep the timescale changed
        public float Duration = 1f;
        /// whether or not the timescale should get lerped
        public bool LerpTimeScale = true;
        /// the speed at which to lerp the timescale
        public float LerpSpeed = 5f;
        /// the cooldown for this ability
        public MMCooldown Cooldown;

        protected bool _timeControlled = false;

        /// <summary>
        /// Watches for input press
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                TimeControlStart();
            }
            if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                TimeControlStop();
            }
        }

        /// <summary>
        /// On initialization, we init our cooldown
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            Cooldown.Initialization();
        }

        /// <summary>
        /// Starts the time scale modification
        /// </summary>
        public virtual void TimeControlStart()
        {
            if (Cooldown.Ready())
            {
                PlayAbilityStartFeedbacks();
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, true);
                Cooldown.Start();
                _timeControlled = true;
            }            
        }

        /// <summary>
        /// Stops the time control
        /// </summary>
        public virtual void TimeControlStop()
        {
            StopStartFeedbacks();
            PlayAbilityStopFeedbacks();
            Cooldown.Stop();
        }

        /// <summary>
        /// On update, we unfreeze time if needed
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            Cooldown.Update();

            if ((Cooldown.CooldownState != MMCooldown.CooldownStates.Consuming) && _timeControlled)
            {
                _timeControlled = false;
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            }
        }
    }
}
