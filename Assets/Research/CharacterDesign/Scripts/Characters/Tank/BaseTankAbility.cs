using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Research.Scripts.Characters.Tank
{
    public abstract class BaseTankAbility : CharacterAbility
    {
        public float cooldownTime;

        public float CurrentTime { get; set; }

        public float CurrentCooldownTime { get; set; }

        public bool BreathButtonDown { get; set; }

        public bool CurrentlyBreathing { get; set; }

        protected bool _used;

        public bool reUsable;

        public bool OnCoolDown
        {
            get
            {
                if (!CurrentlyBreathing)
                {
                    return CurrentCooldownTime > 0.0f;
                }

                return false;
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityPermitted
                || _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                return;
            }

            switch (_inputManager.SecondaryShootButtonState)
            {
                case MMInput.ButtonStates.ButtonDown:
                    CheckStartAbility();
                    break;
                case MMInput.ButtonStates.ButtonUp:
                    StopAbility();
                    break;
            }
        }

        protected abstract void StartAbility();

        protected virtual void CheckStartAbility()
        {
            if (!BreathButtonDown && !CurrentlyBreathing && !OnCoolDown)
            {
                if (reUsable || !_used)
                {
                    StartAbility();
                }
            }
        }


        protected virtual void StopAbility()
        {
            StopBreathingButton();
        }

        protected void StopBreathingButton()
        {
            if (BreathButtonDown)
            {
                BreathButtonDown = false;
            }
        }

        protected void StopBreathingCheck()
        {
            if (CurrentTime <= 0)
            {
                CurrentlyBreathing = false;
                SetWeaponsEnabled(true);
                CurrentCooldownTime = cooldownTime;
            }
        }

        protected void SetWeaponsEnabled(bool setEnabled)
        {
            var weaponAbilitys = GetComponents<CharacterHandleWeapon>();
            foreach (var ability in weaponAbilitys)
            {
                ability.AbilityPermitted = setEnabled;
            }
        }

        protected abstract void UpdateAbility();

        private void Update()
        {
            if (CurrentlyBreathing)
            {
                UpdateAbility();

                CurrentTime -= Time.deltaTime;
                StopBreathingCheck();
            }
            else if (OnCoolDown)
            {
                CurrentCooldownTime -= Time.deltaTime;
            }
        }

        public Dictionary<string, float> GetObservations()
        {
            var breathingFloat = Convert.ToSingle(CurrentlyBreathing);

            var senses = new Dictionary<string, float>()
            {
                { "Breathing Float", breathingFloat },
                { "Current Cool Down", CurrentCooldownTime},
                { "Used", Convert.ToSingle(_used) }
            };

            return senses;
        }

        public string SenseName => "BreatherAbility";
    }
}
