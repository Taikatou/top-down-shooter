using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This Decision will return true if its MMConeOfVision has detected at least one target, and will set it as the Brain's target
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetConeOfVision3D")]
    [RequireComponent(typeof(MMConeOfVision))]
    public class AIDecisionDetectTargetConeOfVision3D : AIDecision
    {
        protected MMConeOfVision _coneOfVision;

        /// <summary>
        /// On Init we grab our MMConeOfVision
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            _coneOfVision = this.gameObject.GetComponent<MMConeOfVision>();
        }

        /// <summary>
        /// On Decide we look for a target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return DetectTarget();
        }

        /// <summary>
        /// If the MMConeOfVision has at least one target, it becomes our new brain target and this decision is true, otherwise it's false.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            if (_coneOfVision.VisibleTargets.Count == 0)
            {
                _brain.Target = null;
                return false;
            }
            else
            {
                _brain.Target = _coneOfVision.VisibleTargets[0];
                return true;
            }
        }
    }
}
