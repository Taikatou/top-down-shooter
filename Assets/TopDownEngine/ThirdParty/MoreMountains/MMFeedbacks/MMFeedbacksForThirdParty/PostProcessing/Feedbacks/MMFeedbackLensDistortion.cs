using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control lens distortion intensity over time. 
    /// It requires you have in your scene an object with a PostProcessVolume 
    /// with Lens Distortion active, and a MMLensDistortionShaker component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Lens Distortion")]
    [FeedbackHelp("This feedback allows you to control lens distortion intensity over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Lens Distortion active, and a MMLensDistortionShaker component.")]
    public class MMFeedbackLensDistortion : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }

        [Header("Lens Distortion")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake in seconds
        public float Duration = 0.5f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Intensity")]
        /// whether or not to add to the initial intensity value
        public bool RelativeIntensity = false;
        /// the curve to animate the intensity on
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0),
                                                                    new Keyframe(0.2f, 1),
                                                                    new Keyframe(0.25f, -1),
                                                                    new Keyframe(0.35f, 0.7f),
                                                                    new Keyframe(0.4f, -0.7f),
                                                                    new Keyframe(0.6f, 0.3f),
                                                                    new Keyframe(0.65f, -0.3f),
                                                                    new Keyframe(0.8f, 0.1f),
                                                                    new Keyframe(0.85f, -0.1f),
                                                                    new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-100f, 100f)]
        public float RemapIntensityZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-100f, 100f)]
        public float RemapIntensityOne = 20f;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return Duration; } }

        /// <summary>
        /// Triggers a lens distortion shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMLensDistortionShakeEvent.Trigger(Intensity, Duration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, attenuation,
                    Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
