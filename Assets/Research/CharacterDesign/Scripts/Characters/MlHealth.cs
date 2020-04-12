using MLAgents;
using MoreMountains.TopDownEngine;
using Research.CharacterDesign.TopDownEngineCustom;
using TopDownEngine.Common.Scripts.Characters.Core;

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
