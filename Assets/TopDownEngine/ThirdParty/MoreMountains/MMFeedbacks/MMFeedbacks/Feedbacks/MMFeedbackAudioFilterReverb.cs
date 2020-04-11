using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you control the reverb level of a reverb filter. You'll need a MMAudioFilterReverbShaker on your filter.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/Audio Filter Reverb")]
    [FeedbackHelp("This feedback lets you control a low pass audio filter over time. You'll need a MMAudioFilterReverbShaker on your filter.")]
    public class MMFeedbackAudioFilterReverb : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        /// returns the duration of the feedback
        public override float FeedbackDuration { get { return Duration; } }

        [Header("Reverb Feedback")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Reverb")]
        /// whether or not to add to the initial value
        public bool RelativeReverb = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve ShakeReverb = new AnimationCurve(new Keyframe(0, 0f), new Keyframe(0.5f, 1f), new Keyframe(1, 0f));
        /// the value to remap the curve's 0 to
        [Range(-10000f, 2000f)]
        public float RemapReverbZero = -10000f;
        /// the value to remap the curve's 1 to
        [Range(-10000f, 2000f)]
        public float RemapReverbOne = 2000f;

        /// <summary>
        /// Triggers the corresponding coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMAudioFilterReverbShakeEvent.Trigger(ShakeReverb, Duration, RemapReverbZero, RemapReverbOne, RelativeReverb,
                    attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
