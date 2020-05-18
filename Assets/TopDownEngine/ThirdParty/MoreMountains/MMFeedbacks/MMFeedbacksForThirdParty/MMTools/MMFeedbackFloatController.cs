using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will trigger a one time play on a target FloatController
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you trigger a one time play on a target FloatController.")]
    [FeedbackPath("GameObject/FloatController")]
    public class MMFeedbackFloatController : MMFeedback
    {
        /// the different possible modes 
        public enum Modes { OneTime, ToDestination }
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.GameObjectColor; } }

        [Header("Float Controller")]
        /// the mode this controller is in
        public Modes Mode = Modes.OneTime;
        /// the float controller to trigger a one time play on
        public FloatController TargetFloatController;
        /// whether this should revert to original at the end
        public bool RevertToInitialValueAfterEnd = false;
        /// the duration of the One Time shake
        [MMFEnumCondition("Mode", (int)Modes.OneTime)]
        public float OneTimeDuration = 1f;
        /// the amplitude of the One Time shake (this will be multiplied by the curve's height)
        [MMFEnumCondition("Mode", (int)Modes.OneTime)]
        public float OneTimeAmplitude = 1f;
        /// the low value to remap the normalized curve value to 
        [MMFEnumCondition("Mode", (int)Modes.OneTime)]
        public float OneTimeRemapMin = 0f;
        /// the high value to remap the normalized curve value to 
        [MMFEnumCondition("Mode", (int)Modes.OneTime)]
        public float OneTimeRemapMax = 1f;
        /// the curve to apply to the one time shake
        [MMFEnumCondition("Mode", (int)Modes.OneTime)]
        public AnimationCurve OneTimeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public float ToDestinationValue = 1f;
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public float ToDestinationDuration = 1f;
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));


        /// the duration of this feedback is the duration of the one time hit
        public override float FeedbackDuration { get { return OneTimeDuration; } }

        protected float _oneTimeDurationStorage;
        protected float _oneTimeAmplitudeStorage;
        protected float _oneTimeRemapMinStorage;
        protected float _oneTimeRemapMaxStorage;
        protected AnimationCurve _oneTimeCurveStorage;
        protected float _toDestinationValueStorage;
        protected float _toDestinationDurationStorage;
        protected AnimationCurve _toDestinationCurveStorage;
        protected bool _revertToInitialValueAfterEndStorage;

        /// <summary>
        /// On init we grab our initial color and components
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            if (Active && (TargetFloatController != null))
            {
                _oneTimeDurationStorage = TargetFloatController.OneTimeDuration;
                _oneTimeAmplitudeStorage = TargetFloatController.OneTimeAmplitude;
                _oneTimeCurveStorage = TargetFloatController.OneTimeCurve;
                _oneTimeRemapMinStorage = TargetFloatController.OneTimeRemapMin;
                _oneTimeRemapMaxStorage = TargetFloatController.OneTimeRemapMax;
                _toDestinationCurveStorage = TargetFloatController.ToDestinationCurve;
                _toDestinationDurationStorage = TargetFloatController.ToDestinationDuration;
                _toDestinationValueStorage = TargetFloatController.ToDestinationValue;
                _revertToInitialValueAfterEndStorage = TargetFloatController.RevertToInitialValueAfterEnd;
            }
        }

        /// <summary>
        /// On play we make our renderer flicker
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (TargetFloatController != null))
            {
                TargetFloatController.RevertToInitialValueAfterEnd = RevertToInitialValueAfterEnd;

                if (Mode == Modes.OneTime)
                {
                    TargetFloatController.OneTimeDuration = OneTimeDuration;
                    TargetFloatController.OneTimeAmplitude = OneTimeAmplitude;
                    TargetFloatController.OneTimeCurve = OneTimeCurve;
                    TargetFloatController.OneTimeRemapMin = OneTimeRemapMin;
                    TargetFloatController.OneTimeRemapMax = OneTimeRemapMax;
                    TargetFloatController.OneTime();
                }
                if (Mode == Modes.ToDestination)
                {
                    TargetFloatController.ToDestinationCurve = ToDestinationCurve;
                    TargetFloatController.ToDestinationDuration = ToDestinationDuration;
                    TargetFloatController.ToDestinationValue = ToDestinationValue;
                    TargetFloatController.ToDestination();
                }
            }
        }

        /// <summary>
        /// On reset we make our renderer stop flickering
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();
            if (Active && (TargetFloatController != null))
            {
                TargetFloatController.OneTimeDuration = _oneTimeDurationStorage;
                TargetFloatController.OneTimeAmplitude = _oneTimeAmplitudeStorage;
                TargetFloatController.OneTimeCurve = _oneTimeCurveStorage;
                TargetFloatController.OneTimeRemapMin = _oneTimeRemapMinStorage;
                TargetFloatController.OneTimeRemapMax = _oneTimeRemapMaxStorage;
                TargetFloatController.ToDestinationCurve = _toDestinationCurveStorage;
                TargetFloatController.ToDestinationDuration = _toDestinationDurationStorage;
                TargetFloatController.ToDestinationValue = _toDestinationValueStorage;
                TargetFloatController.RevertToInitialValueAfterEnd = _revertToInitialValueAfterEndStorage;
            }
        }

    }
}
