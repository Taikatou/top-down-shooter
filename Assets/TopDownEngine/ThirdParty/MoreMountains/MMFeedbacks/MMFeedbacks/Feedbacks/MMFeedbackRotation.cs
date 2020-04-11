using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback animates the rotation of the specified object when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will animate the target's rotation on the 3 specified animation curves (one per axis), for the specified duration (in seconds).")]
    [FeedbackPath("Transform/Rotation")]
    public class MMFeedbackRotation : MMFeedback
    {
        /// the possible modes for this feedback (Absolute : always follow the curve from start to finish, Additive : add to the values found when this feedback gets played)
        public enum Modes { Absolute, Additive, ToDestination }
        /// the timescale modes this feedback can operate on
        public enum TimeScales { Scaled, Unscaled }

        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.TransformColor; } }

        [Header("Rotation Target")]
        /// the object whose rotation you want to animate
        public Transform AnimateRotationTarget;

        [Header("Animation")]
        /// whether this feedback should animate in absolute values or additive
        public Modes Mode = Modes.Absolute;
        /// whether this feedback should play in scaled or unscaled time
        public TimeScales TimeScale = TimeScales.Scaled;
        /// the duration of the transition
        public float AnimateRotationDuration = 0.2f;
        /// the value to remap the curve's 0 value to
        public float RemapCurveZero = 0f;
        /// the value to remap the curve's 1 value to
        [FormerlySerializedAs("Multiplier")]
        public float RemapCurveOne = 360f;
        /// if this is true, should animate the X rotation
        public bool AnimateX = true;
        /// how the x part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateX", true)]
        public AnimationCurve AnimateRotationX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// if this is true, should animate the X rotation
        public bool AnimateY = true;
        /// how the y part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateY", true)]
        public AnimationCurve AnimateRotationY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// if this is true, should animate the X rotation
        public bool AnimateZ = true;
        /// how the z part of the rotation should animate over time, in degrees
        [MMFCondition("AnimateZ", true)]
        public AnimationCurve AnimateRotationZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));

        [Header("To Destination")]
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public Space ToDestinationSpace = Space.World;
        [MMFEnumCondition("Mode", (int)Modes.ToDestination)]
        public Vector3 DestinationAngles = new Vector3(0f, 180f, 0f);

        /// the duration of this feedback is the duration of the rotation
        public override float FeedbackDuration { get { return AnimateRotationDuration; } }

        protected Quaternion _initialRotation;
        protected Vector3 _up;
        protected Vector3 _right;
        protected Vector3 _forward;
        protected Quaternion _destinationRotation;

        /// <summary>
        /// On init we store our initial rotation
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            if (Active && (AnimateRotationTarget != null))
            {
                _initialRotation = AnimateRotationTarget.rotation;
            }
        }

        /// <summary>
        /// On play, we trigger our rotation animation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (AnimateRotationTarget != null))
            {
                if (isActiveAndEnabled)
                {
                    if ((Mode == Modes.Absolute) || (Mode == Modes.Additive))
                    {
                        StartCoroutine(AnimateRotation(AnimateRotationTarget, Vector3.zero, AnimateRotationDuration, AnimateRotationX, AnimateRotationY, AnimateRotationZ, RemapCurveZero, RemapCurveOne));
                    }
                    if (Mode == Modes.ToDestination)
                    {
                        StartCoroutine(RotateToDestination());
                    }
                }
            }
        }

        /// <summary>
        /// A coroutine used to rotate the target to its destination rotation
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator RotateToDestination()
        {
            if (AnimateRotationTarget == null)
            {
                yield break;
            }

            if ((AnimateRotationX == null) || (AnimateRotationY == null) || (AnimateRotationZ == null))
            {
                yield break;
            }

            if (AnimateRotationDuration == 0f)
            {
                yield break;
            }

            float journey = 0f;

            _initialRotation = AnimateRotationTarget.transform.rotation;
            if (ToDestinationSpace == Space.Self)
            {
                AnimateRotationTarget.transform.localRotation = Quaternion.Euler(DestinationAngles);
            }
            else
            {
                AnimateRotationTarget.transform.rotation = Quaternion.Euler(DestinationAngles);
            }
            
            _destinationRotation = AnimateRotationTarget.transform.rotation;
            AnimateRotationTarget.transform.rotation = _initialRotation;

            while (journey < AnimateRotationDuration)
            {
                float percent = Mathf.Clamp01(journey / AnimateRotationDuration);

                Quaternion newRotation = Quaternion.LerpUnclamped(_initialRotation, _destinationRotation, percent);
                AnimateRotationTarget.transform.rotation = newRotation;

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            if (ToDestinationSpace == Space.Self)
            {
                AnimateRotationTarget.transform.localRotation = Quaternion.Euler(DestinationAngles);
            }
            else
            {
                AnimateRotationTarget.transform.rotation = Quaternion.Euler(DestinationAngles);
            }

            yield break;
        }

        /// <summary>
        /// A coroutine used to compute the rotation over time
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="vector"></param>
        /// <param name="duration"></param>
        /// <param name="curveX"></param>
        /// <param name="curveY"></param>
        /// <param name="curveZ"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        protected virtual IEnumerator AnimateRotation(Transform targetTransform,
                                                    Vector3 vector,
                                                    float duration,
                                                    AnimationCurve curveX,
                                                    AnimationCurve curveY,
                                                    AnimationCurve curveZ,
                                                    float remapZero,
                                                    float remapOne)
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

            if (Mode == Modes.Additive)
            {
                _initialRotation = targetTransform.rotation;
            }
            
            _up = targetTransform.up;
            _right = targetTransform.right;
            _forward = targetTransform.forward;

            while (journey < duration)
            {
                float percent = Mathf.Clamp01(journey / duration);
                
                ApplyRotation(targetTransform, remapZero, remapOne, curveX, curveY, curveZ, percent);

                journey += (TimeScale == TimeScales.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime;
                yield return null;
            }

            ApplyRotation(targetTransform, remapZero, remapOne, curveX, curveY, curveZ, 1f);

            yield break;
        }

        /// <summary>
        /// Computes and applies the rotation to the object
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="multiplier"></param>
        /// <param name="curveX"></param>
        /// <param name="curveY"></param>
        /// <param name="curveZ"></param>
        /// <param name="percent"></param>
        protected virtual void ApplyRotation(Transform targetTransform, float remapZero, float remapOne, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float percent)
        {
            targetTransform.transform.rotation = _initialRotation;

            if (AnimateX)
            {
                float x = curveX.Evaluate(percent);
                x = MMFeedbacksHelpers.Remap(x, 0f, 1f, remapZero, remapOne);
                targetTransform.Rotate(_right, x);
            }
            if (AnimateY)
            {
                float y = curveY.Evaluate(percent);
                y = MMFeedbacksHelpers.Remap(y, 0f, 1f, remapZero, remapOne);
                targetTransform.Rotate(_up, y);
            }
            if (AnimateZ)
            {
                float z = curveZ.Evaluate(percent);
                z = MMFeedbacksHelpers.Remap(z, 0f, 1f, remapZero, remapOne);
                targetTransform.Rotate(_forward, z);
            }
        }
    }
}
