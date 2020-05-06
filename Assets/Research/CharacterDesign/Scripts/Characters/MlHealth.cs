using MLAgents;
using MoreMountains.TopDownEngine;
using Research.Common;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlHealth : Health
    {
        public override void Kill()
        {
            base.Kill();
            showHealth = true;
            
            var requester = GetComponent<DecisionRequester>();

            if (requester)
            {
                Academy.Instance.AgentPreStep -= requester.MakeRequests;   
            }
            
            // delete bullets
            var pools = GetComponentsInChildren<MLObjectPooler>();
            foreach (var pool in pools)
            {
                pool.DestroySpawnable();
            }
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
