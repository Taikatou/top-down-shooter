using MoreMountains.TopDownEngine;
using Research.Common;
using Unity.MLAgents;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlHealth : Health
    {
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
        }

        public override void Revive()
        {
            base.Revive();
            ResetHealthToMaxHealth();
        }
        
        public bool showHealth = false;

        private void Update()
        {
            if (showHealth)
            {
                CurrentHealth = InitialHealth;
                showHealth = false;
                Invulnerable = false;
            }
        }
    }
}
