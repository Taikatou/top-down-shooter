using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will animate the scale of the target object over time when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Transform/Scale")]
    [FeedbackHelp("This feedback will animate the target's scale on the 3 specified animation curves, for the specified duration (in seconds). You can apply a multiplier, that will multiply each animation curve value.")]
    public class MMFeedbackScale : MMFeedback
    {
        /// the possible modes this feedback can operate on
        public enum Modes { Absolute, Additive, ToDestination }
        /// the possible timescales for the animation of the scale
        public enum TimeScales { Scaled, Unscaled }
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.TransformColor; } }

        [Header("Scale")]
        /// the mode this feedback should operate on
        /// Absolute : follows the curve
        /// Additive : adds to the current scale of the target
        /// ToDestination : sets the scale to the destination target, whatever the current scale is
        public Modes Mode = Modes.Absolute;
        /// whether this feedback should play in scaled or unscaled time
        public TimeScales TimeScale = TimeScales.Scaled;
        /// the object to animate
        public Transform AnimateScaleTarget;
        /// the duration of the animation
        public float AnimateScaleDuration = 0.2f;
        /// the value to remap the curve's 0 value to
        public float RemapCurveZero = 1f;
        /// the value to remap the curve's 1 value to
        [FormerlySerializedAs("Multiplier")]
        public float RemapCurveOne = 2f;
        /// how much should be added to the curve
        public float Offset = 0f;
        /// if this is true, should animate the X scale value
        public bool AnimateX = true;
        /// the x scale animation definition
        [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimateScaleX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0));
        /// if this is true, should animate the Y scale value
        public bool AnimateY = true;
        /// the y scale animation definition
        [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimateScaleY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0));
        /// if this is true, should animate the z scale value
        public bool AnimateZ = true;
        /// the z scale animation definition
        [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimateScaleZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.5f), new Keyframe(1, 0));

        [Header("To Destination")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public Vector3 DestinationScale = new Vector3(0.5f, 0.5f, 0.5f);

        /// the duration of this feedback is the duration of the scale animation
        public override float FeedbackDuration { get { return AnimateScaleDuration; } }

        protected Vector3 _initialScale;
        protected Vector3 _newScale;

        /// <summary>
        /// On init we store our initial scale
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active && (AnimateScaleTarget != null))
            {
                _initialScale = AnimateScaleTarget.localScale;
            }
        }

        /// <summary>
        /// On Play, triggers the scale animation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimateScaleTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    if ((Mode == Modes.Absolute) || (Mode == Modes.Additive))
                    {
                        StartCoroutine(AnimateScale(AnimateScaleTarget, Vector3.zero, AnimateScaleDuration, AnimateScaleX, AnimateScaleY, AnimateScaleZ, RemapCurveZero, RemapCurveOne));
                    }
                    if (Mode == Modes.ToDestination)
                    {
                        StartCoroutine(ScaleToDestination());
                    }                    
                }
            }
        }

        /// <summary>
        /// An internal coroutine used to scale the target to its destination scale
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ScaleToDestination()
        {
            if (AnimateScaleTarget == null)
            {
                yield break;
            }

            if ((AnimateScaleX == null) || (AnimateScaleY == null) || (AnimateScaleZ == null))
            {
                yield break;
            }

            if (AnimateScaleDuration == 0f)
            {
                yield break;
            }

            float journey = 0f;

            _initialScale = AnimateScaleTarget.localScale;

            while (journey < AnimateScaleDuration)
            {
                float percent = Mathf.Clamp01(journey / AnimateScaleDuration);

                _newScale.x = Mathf.LerpUnclamped(_initialScale.x, DestinationScale.x, AnimateScaleX.Evaluate(percent) + Offset);
                _newScale.y = Mathf.LerpUnclamped(_initialScale.y, DestinationScale.y, AnimateScaleY.Evaluate(percent) + Offset);
                _newScale.z = Mathf.LerpUnclamped(_initialScale.z, DestinationScale.z, AnimateScaleZ.Evaluate(percent) + Offset);

                _newScale.x = MMFeedbacksHelpers.Remap(_newScale.x, 0f, 1f, RemapCurveZero, RemapCurveOne);
                _newScale.y = MMFeedbacksHelpers.Remap(_newScale.y, 0f, 1f, RemapCurveZero, RemapCurveOne);
                _newScale.z = MMFeedbacksHelpers.Remap(_newScale.z, 0f, 1f, RemapCurveZero, RemapCurveOne);

                AnimateScaleTarget.localScale = _newScale;

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            AnimateScaleTarget.localScale = DestinationScale;

            yield return null;
        }

        /// <summary>
        /// An internal coroutine used to animate the scale over time
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="vector"></param>
        /// <param name="duration"></param>
        /// <param name="curveX"></param>
        /// <param name="curveY"></param>
        /// <param name="curveZ"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        protected virtual IEnumerator AnimateScale(Transform targetTransform, Vector3 vector, float duration, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float remapCurveZero = 0f, float remapCurveOne = 1f)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            if ((curveX == null) || (curveY == null) || (curveZ == null))
            {
                yield break;
            }

            if (duration == 0f)
            {
                yield break;
            }

            float journey = 0f;
            
            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);

                vector.x = AnimateX ? curveX.Evaluate(percent) + Offset : targetTransform.localScale.x;
                vector.y = AnimateY ? curveY.Evaluate(percent) + Offset : targetTransform.localScale.y;
                vector.z = AnimateZ ? curveZ.Evaluate(percent) + Offset : targetTransform.localScale.z;

                vector.x = MMFeedbacksHelpers.Remap(vector.x, 0f, 1f, RemapCurveZero, RemapCurveOne);
                vector.y = MMFeedbacksHelpers.Remap(vector.y, 0f, 1f, RemapCurveZero, RemapCurveOne);
                vector.z = MMFeedbacksHelpers.Remap(vector.z, 0f, 1f, RemapCurveZero, RemapCurveOne);
                
                if (Mode == Modes.Additive)
                {
                    vector += _initialScale;
                }

                targetTransform.localScale = vector;

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            vector.x = AnimateX ? curveX.Evaluate(1f) + Offset : targetTransform.localScale.x;
            vector.y = AnimateY ? curveY.Evaluate(1f) + Offset : targetTransform.localScale.y;
            vector.z = AnimateZ ? curveZ.Evaluate(1f) + Offset : targetTransform.localScale.z;
                       
            vector.x = MMFeedbacksHelpers.Remap(vector.x, 0f, 1f, RemapCurveZero, RemapCurveOne);
            vector.y = MMFeedbacksHelpers.Remap(vector.y, 0f, 1f, RemapCurveZero, RemapCurveOne);
            vector.z = MMFeedbacksHelpers.Remap(vector.z, 0f, 1f, RemapCurveZero, RemapCurveOne);

            if (Mode == Modes.Additive)
            {
                vector += _initialScale;
            }
            targetTransform.localScale = vector;

            yield return null;
        }
    }
}
