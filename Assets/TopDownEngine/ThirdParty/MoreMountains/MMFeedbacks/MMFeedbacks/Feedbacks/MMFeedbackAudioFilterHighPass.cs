using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you control the cutoff frequency of a high pass filter. You'll need a MMAudioFilterHighPassShaker on your filter.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/Audio Filter High Pass")]
    [FeedbackHelp("This feedback lets you control a high pass audio filter over time. You'll need a MMAudioFilterHighPassShaker on your filter.")]
    public class MMFeedbackAudioFilterHighPass : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        /// returns the duration of the feedback
        public override float FeedbackDuration { get { return Duration; } }

        [Header("High Pass Feedback")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("High Pass")]
        /// whether or not to add to the initial value
        public bool RelativeHighPass = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve ShakeHighPass = new AnimationCurve(new Keyframe(0, 0f), new Keyframe(0.5f, 1f), new Keyframe(1, 0f));
        /// the value to remap the curve's 0 to
        [Range(10f, 22000f)]
        public float RemapHighPassZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(10f, 22000f)]
        public float RemapHighPassOne = 10000f;

        /// <summary>
        /// Triggers the corresponding coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMAudioFilterHighPassShakeEvent.Trigger(ShakeHighPass, Duration, RemapHighPassZero, RemapHighPassOne, RelativeHighPass,
                    attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
