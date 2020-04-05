using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will trigger a post processing moving filter event, meant to be caught by a MMPostProcessingMovableFilter object
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will trigger a post processing moving filter event, meant to be caught by a MMPostProcessingMovableFilter object")]
    [FeedbackPath("PostProcess/PPMovingFilter")]
    public class MMFeedbackPPMovingFilter : MMFeedback
    {
        /// the possible modes for this feedback 
        public enum Modes { Toggle, On, Off }

        [Header("PostProcessing Profile Moving Filter")]
        /// the selected mode for this feedback 
        public Modes Mode = Modes.Toggle;
        /// the channel to target
        public int Channel = 0;
        /// the duration of the transition
        public float TransitionDuration = 1f;
        /// the curve to move along to
        public MMTween.MMTweenCurve Curve = MMTween.MMTweenCurve.EaseInCubic;

        protected bool _active = false;
        protected bool _toggle = false;

        /// <summary>
        /// On custom play, we trigger a MMPostProcessingMovingFilterEvent with the selected parameters
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                _active = (Mode == Modes.On);
                _toggle = (Mode == Modes.Toggle);

                MMPostProcessingMovingFilterEvent.Trigger(Curve, _active, _toggle, TransitionDuration, Channel);
            }
        }
    }
}
