using MoreMountains.TopDownEngine;
using Research.CharacterDesign.Scripts.Environment;
using Research.Common;
using Unity.MLAgents;
using UnityEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlHealth : Health
    {
        public DecisionRequester decisionRequester;
        public bool showHealth = false;
        private string _oldTag;
        public TopDownInputManager inputManager;
        protected override void Start()
        {
            base.Start();
            _character.ConditionState.OnStateChange += () =>
            {
                if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
                {
                    _oldTag = _character.gameObject.tag;
                    _character.gameObject.tag = "Walls";
                }
                else
                {
                    _character.gameObject.tag = _oldTag;
                }
            };
        }
        public void AddHealth(float health)
        {
            CurrentHealth += health;
            if (CurrentHealth > MaximumHealth)
            {
                CurrentHealth = MaximumHealth;
            }
        }
        public override void Kill()
        {
            showHealth = true;
            CurrentHealth = 0;

            // delete bullets
            var pools = GetComponentsInChildren<MLObjectPooler>();
            foreach (var pool in pools)
            {
                pool.DestroySpawnable();
            }
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Dead);
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.PlayerDeath, _character);

            decisionRequester.allowDecisions = false;

            DisableActions();
        }

        private void DisableActions()
        {
            if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
            {
                inputManager.SetAiPrimaryMovement(new Vector2(0, 0));
                inputManager.SetShootButton(false);
                inputManager.SetSecondaryShootButton(false);
            }
        }

        public override void Revive()
        {
            base.Revive();
            ResetHealthToMaxHealth();
            
            decisionRequester.allowDecisions = true;
        }

        private void Update()
        {
            DisableActions();
            if (showHealth)
            {
                CurrentHealth = InitialHealth;
                showHealth = false;
                Invulnerable = false;
            }
        }
    }
}
