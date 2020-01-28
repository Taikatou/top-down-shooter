using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleResearch.Scripts
{
    public class BreatherAbility : CharacterAbility, ISense
    {
        public float BreathTime;

        public float HeathBack;

        public float CooldownTime;

        public float CurrentTime { get; set; }

        public float CurrentCooldownTime { get; set; }

        public bool BreathButtonDown { get; set; }

        public bool CurrentlyBreathing { get; set; }

        private Health HealthComponent => GetComponent<Health>();

        private bool _used;

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
                    StartBreath();
                    break;
                case MMInput.ButtonStates.ButtonUp:
                    StopBreathingButton();
                    break;
            }
        }

        protected void StartBreath()
        {
            if (!BreathButtonDown && !CurrentlyBreathing && !OnCoolDown)
            {
                if (reUsable || !_used)
                {
                    if (HealthComponent.CurrentHealth < HealthComponent.MaximumHealth)
                    {
                        BreathButtonDown = true;
                        CurrentlyBreathing = true;
                        CurrentTime = BreathTime;
                        SetWeaponsEnabled(false);
                    
                        _used = true;
                    }
                }
            }
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
                CurrentCooldownTime = CooldownTime;
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

        private void Update()
        {
            if (CurrentlyBreathing)
            {
                var healthBack = (HeathBack / BreathTime) * Time.deltaTime;
                HealthComponent?.GetHealth(healthBack, gameObject);

                CurrentTime -= Time.deltaTime;
                StopBreathingCheck();
            }
            else if(OnCoolDown)
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
