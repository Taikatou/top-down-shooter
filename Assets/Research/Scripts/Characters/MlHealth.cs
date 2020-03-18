﻿using MLAgents;
using MoreMountains.TopDownEngine;

namespace Research.Scripts.Characters
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