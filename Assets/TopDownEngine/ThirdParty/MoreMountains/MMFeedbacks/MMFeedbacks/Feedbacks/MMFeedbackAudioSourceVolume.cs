using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you control the volume of a target AudioSource over time.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/AudioSource Volume")]
    [FeedbackHelp("This feedback lets you control the volume of a target AudioSource over time.")]
    public class MMFeedbackAudioSourceVolume : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
        /// returns the duration of the feedback
        public override float FeedbackDuration { get { return Duration; } }

        [Header("Volume Feedback")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Volume")]
        /// whether or not to add to the initial value
        public bool RelativeVolume = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve VolumeTween = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(0.5f, 0f), new Keyframe(1, 1f));
        /// the value to remap the curve's 0 to
        [Range(0f, 1f)]
        public float RemapVolumeZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(0f, 1f)]
        public float RemapVolumeOne = 1f;


        /// <summary>
        /// Triggers the corresponding coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMAudioSourceVolumeShakeEvent.Trigger(VolumeTween, Duration, RemapVolumeZero, RemapVolumeOne, RelativeVolume, 
                    attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
