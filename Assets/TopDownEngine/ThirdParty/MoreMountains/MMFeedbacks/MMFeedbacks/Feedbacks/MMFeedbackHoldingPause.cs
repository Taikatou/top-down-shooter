using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// this feedback will "hold", or wait, until all previous feedbacks have been executed, and will then pause the execution of your MMFeedbacks sequence, for the specified duration
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will 'hold', or wait, until all previous feedbacks have been executed, and will then pause the execution of your MMFeedbacks sequence, for the specified duration.")]
    [FeedbackPath("Pause/Holding Pause")]
    public class MMFeedbackHoldingPause : MMFeedbackPause
    {
        /// sets the color of this feedback in the inspector
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.HoldingPauseColor; } }
        public override bool HoldingPause { get { return true; } }
        public override YieldInstruction Pause { get { return _waitForSeconds; } }
                
        /// the duration of this feedback is the duration of the pause
        public override float FeedbackDuration { get { return PauseDuration; } }
        
        /// <summary>
        /// On custom play we just play our pause
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                StartCoroutine(PlayPause());
            }
        }
    }
}
