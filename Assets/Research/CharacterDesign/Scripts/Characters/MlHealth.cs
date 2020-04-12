using MLAgents;
using MoreMountains.TopDownEngine;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlHealth : Health
    {
        public override void Kill()
        {
            base.Kill();
            
            var requester = GetComponent<DecisionRequester>();
            
            Academy.Instance.AgentPreStep -= requester.MakeRequests;
        }
    }
}
