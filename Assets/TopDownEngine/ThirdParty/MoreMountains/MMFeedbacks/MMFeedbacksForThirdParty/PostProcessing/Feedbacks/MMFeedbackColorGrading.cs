using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control color grading post exposure, hue shift, saturation and contrast over time. 
    /// It requires you have in your scene an object with a PostProcessVolume 
    /// with Color Grading active, and a MMColorGradingShaker component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Color Grading")]
    [FeedbackHelp("This feedback allows you to control color grading post exposure, hue shift, saturation and contrast over time. " +
            "It requires you have in your scene an object with a PostProcessVolume " +
            "with Color Grading active, and a MMColorGradingShaker component.")]
    public class MMFeedbackColorGrading : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }
        
        [Header("Color Grading")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float ShakeDuration = 1f;
        /// whether or not to add to the initial intensity
        public bool RelativeIntensity = true;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Post Exposure")]
        /// the curve used to animate the focus distance value on
        public AnimationCurve ShakePostExposure = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        public float RemapPostExposureZero = 0f;
        /// the value to remap the curve's 1 to
        public float RemapPostExposureOne = 1f;

        [Header("Hue Shift")]
        /// the curve used to animate the aperture value on
        public AnimationCurve ShakeHueShift = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-180f, 180f)]
        public float RemapHueShiftZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-180f, 180f)]
        public float RemapHueShiftOne = 180f;

        [Header("Saturation")]
        /// the curve used to animate the focal length value on
        public AnimationCurve ShakeSaturation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-100f, 100f)]
        public float RemapSaturationZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-100f, 100f)]
        public float RemapSaturationOne = 100f;

        [Header("Contrast")]
        /// the curve used to animate the focal length value on
        public AnimationCurve ShakeContrast = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-100f, 100f)]
        public float RemapContrastZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-100f, 100f)]
        public float RemapContrastOne = 100f;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return ShakeDuration; } }

        /// <summary>
        /// Triggers a color grading shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMColorGradingShakeEvent.Trigger(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, 
                    ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, 
                    ShakeSaturation, RemapSaturationZero, RemapSaturationOne, 
                    ShakeContrast, RemapContrastZero, RemapContrastOne, 
                    ShakeDuration,                     
                    RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
