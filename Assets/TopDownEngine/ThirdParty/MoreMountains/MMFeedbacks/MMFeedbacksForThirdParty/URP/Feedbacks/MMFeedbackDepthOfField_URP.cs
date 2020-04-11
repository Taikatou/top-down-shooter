using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control URP depth of field focus distance, aperture and focal length over time. 
    /// It requires you have in your scene an object with a Volume 
    /// with Depth of Field active, and a MMDepthOfFieldShaker_URP component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to control URP depth of field focus distance, aperture and focal length over time. " +
            "It requires you have in your scene an object with a Volume " +
            "with Depth of Field active, and a MMDepthOfFieldShaker_URP component.")]
    [FeedbackPath("PostProcess/Depth Of Field URP")]
    public class MMFeedbackDepthOfField_URP : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }

        [Header("Depth Of Field")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float ShakeDuration = 2f;
        /// whether or not to add to the initial values
        public bool RelativeValues = true;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Focus Distance")]
        /// the curve used to animate the focus distance value on
        public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        public float RemapFocusDistanceZero = 0f;
        /// the value to remap the curve's 1 to
        public float RemapFocusDistanceOne = 3f;

        [Header("Aperture")]
        /// the curve used to animate the aperture value on
        public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(0.1f, 32f)]
        public float RemapApertureZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(0.1f, 32f)]
        public float RemapApertureOne = 0f;

        [Header("Focal Length")]
        /// the curve used to animate the focal length value on
        public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(0f, 300f)]
        public float RemapFocalLengthZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(0f, 300f)]
        public float RemapFocalLengthOne = 0f;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return ShakeDuration; } }

        /// <summary>
        /// Triggers a depth of field event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMDepthOfFieldShakeEvent_URP.Trigger(ShakeFocusDistance, ShakeDuration, RemapFocusDistanceZero, RemapFocusDistanceOne,
                    ShakeAperture, RemapApertureZero, RemapApertureOne,
                    ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne,
                    RelativeValues, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
