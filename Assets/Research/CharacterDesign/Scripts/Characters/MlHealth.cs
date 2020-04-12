using MLAgents;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.TopDownEngineCustom;

namespace Research.CharacterDesign.Scripts.Characters
{
    public class MlHealth : Health
    {
        public override void Kill()
        {
            base.Kill();
            
            var requester = GetComponent<DecisionRequester>();
            
            requester.allowDecisions = false;
        }
    }
}
