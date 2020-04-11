using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you control the distortion level of a distortion filter. You'll need a MMAudioFilterDistortionShaker on the filter.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/Audio Filter Distortion")]
    [FeedbackHelp("This feedback lets you control a distortion audio filter over time. You'll need a MMAudioFilterDistortionShaker on the filter.")]
    public class MMFeedbackAudioFilterDistortion : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        /// returns the duration of the feedback
        public override float FeedbackDuration { get { return Duration; } }

        [Header("Distortion Feedback")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Distortion")]
        /// whether or not to add to the initial value
        public bool RelativeDistortion = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve ShakeDistortion = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(0f, 1f)]
        public float RemapDistortionZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(0f, 1f)]
        public float RemapDistortionOne = 1f;

        /// <summary>
        /// Triggers the corresponding coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMAudioFilterDistortionShakeEvent.Trigger(ShakeDistortion, Duration, RemapDistortionZero, RemapDistortionOne, RelativeDistortion,
                    attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
