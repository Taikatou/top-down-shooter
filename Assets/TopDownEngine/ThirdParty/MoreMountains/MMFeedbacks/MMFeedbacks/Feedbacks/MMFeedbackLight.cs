﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will let you control the color and intensity of a Light when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback lets you control the color and intensity of a Light in your scene for a certain duration (or instantly).")]
    [FeedbackPath("Light")]
    public class MMFeedbackLight : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.LightColor; } }

        /// the possible modes for this feedback
        public enum Modes { OverTime, Instant}

        [Header("Light")]
        /// the light to affect when playing the feedback
        public Light BoundLight;
        /// whether the feedback should affect the light instantly or over a period of time
        public Modes Mode = Modes.OverTime;
        /// the colors to apply to the light over time
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public Gradient ColorOverTime;
        /// the intensity to apply to the light over time
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.05f), new Keyframe(1, 0));
        /// how long the light should change over time
        [MMFEnumCondition("Mode", (int)Modes.OverTime)]
        public float Duration = 0.2f;
        /// how much that intensity should be multiplied by
        public float IntensityMultiplier = 1.0f;
        /// whether or not that light should be turned off on start
        public bool StartsOff = true;


        /// the duration of this feedback is the duration of the light, or 0 if instant
        public override float FeedbackDuration { get { return (Mode == Modes.Instant) ? 0f : Duration; } }

        /// <summary>
        /// On init we turn the light off if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);

            if (Active)
            {
                if (StartsOff)
                {
                    Turn(false);
                }
            }
        }

        /// <summary>
        /// On Play we turn our light on and start an over time coroutine if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                Turn(true);
                if (Mode == Modes.OverTime)
                {
                    StartCoroutine(LightSequence());
                }
            }
        }

        /// <summary>
        /// This coroutine will modify the intensity and color of the light over time
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator LightSequence()
        {
            float journey = 0f;
            while (journey < Duration)
            {
                float percent = Mathf.Clamp01(journey / Duration);

                BoundLight.intensity = Intensity.Evaluate(percent) * IntensityMultiplier;
                BoundLight.color = ColorOverTime.Evaluate(percent);

                journey += FeedbackDeltaTime;
                yield return null;
            }
            Turn(false);
            yield return null;
        }

        /// <summary>
        /// Turns the light off on stop
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);
            if (Active)
            {
                Turn(false);
            }
        }

        /// <summary>
        /// Turns the light on or off
        /// </summary>
        /// <param name="status"></param>
        protected virtual void Turn(bool status)
        {
            BoundLight.gameObject.SetActive(status);
            BoundLight.enabled = status;
        }
    }
}
