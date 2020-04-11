﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control URP motion blur intensity over time.
    /// It requires you have in your scene an object with a Volume 
    /// with MotionBlur active, and a MMMotionBlurShaker_URP component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Motion Blur URP")]
    [FeedbackHelp("This feedback allows you to control motion blur intensity over time. " +
            "It requires you have in your scene an object with a Volume " +
            "with MotionBlur active, and a MMMotionBlurShaker_URP component.")]
    public class MMFeedbackMotionBlur_URP : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }

        [Header("Motion Blur")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 0.2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Intensity")]
        /// the curve to animate the intensity on
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the multiplier to apply to the intensity curve
        [Range(0f, 1f)]
        public float RemapIntensityZero = 0f;
        [Range(0f, 1f)]
        public float RemapIntensityOne = 1.0f;
        /// whether or not to add to the initial intensity
        public bool RelativeIntensity = false;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return Duration; } }

        /// <summary>
        /// Triggers a motion blur shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMMotionBlurShakeEvent_URP.Trigger(Intensity, Duration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, attenuation,
                    Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
