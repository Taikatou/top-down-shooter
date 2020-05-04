using MLAgents;
using MoreMountains.TopDownEngine;

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
