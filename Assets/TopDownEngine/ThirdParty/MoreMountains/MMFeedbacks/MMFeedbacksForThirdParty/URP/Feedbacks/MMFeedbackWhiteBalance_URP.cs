using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control white balance temperature and tint over time. 
    /// It requires you have in your scene an object with a Volume with White Balance active, and a MMWhiteBalanceShaker_URP component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to control white balance temperature and tint over time. " +
        "It requires you have in your scene an object with a Volume " +
            "with WhiteBalance active, and a MMWhiteBalanceShaker_URP component.")]
    [FeedbackPath("PostProcess/White Balance URP")]
    public class MMFeedbackWhiteBalance_URP : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }

        [Header("White Balance")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float ShakeDuration = 1f;
        /// whether or not to add to the initial value
        public bool RelativeValues = true;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Temperature")]
        /// the curve used to animate the temperature value on
        public AnimationCurve ShakeTemperature = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-100f, 100f)]
        public float RemapTemperatureZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-100f, 100f)]
        public float RemapTemperatureOne = 100f;

        [Header("Tint")]
        /// the curve used to animate the tint value on
        public AnimationCurve ShakeTint = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(-100f, 100f)]
        public float RemapTintZero = 0f;
        /// the value to remap the curve's 1 to
        [Range(-100f, 100f)]
        public float RemapTintOne = 100f;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return ShakeDuration; } }

        /// <summary>
        /// Triggers a white balance shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMWhiteBalanceShakeEvent_URP.Trigger(ShakeTemperature, ShakeDuration, RemapTemperatureZero, RemapTemperatureOne,
                    ShakeTint, RemapTintZero, RemapTintOne, RelativeValues, attenuation,
                    Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
