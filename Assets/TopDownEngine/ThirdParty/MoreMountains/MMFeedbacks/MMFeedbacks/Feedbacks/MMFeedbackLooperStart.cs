using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback can act as a pause but also as a start point for your loops. Add a FeedbackLooper below this (and after a few feedbacks) and your MMFeedbacks will loop between both
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback can act as a pause but also as a start point for your loops. Add a FeedbackLooper below this (and after a few feedbacks) and your MMFeedbacks will loop between both.")]
    [FeedbackPath("Loop/Looper Start")]
    public class MMFeedbackLooperStart : MMFeedbackPause
    {
        /// sets the color of this feedback in the inspector
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.LooperStartColor; } }
        public override bool LooperStart { get { return true; } }
        public override YieldInstruction Pause { get { return _waitForSeconds; } }

        /// the duration of this feedback is the duration of the pause
        public override float FeedbackDuration { get { return PauseDuration; } }

        /// <summary>
        /// Overrides the default value
        /// </summary>
        protected virtual void Reset()
        {
            PauseDuration = 0;
        }

        /// <summary>
        /// On play we run our pause
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
