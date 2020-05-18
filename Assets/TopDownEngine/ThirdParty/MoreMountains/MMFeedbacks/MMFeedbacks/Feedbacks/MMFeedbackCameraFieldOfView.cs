using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback lets you control a camera's field of view over time. You'll need a MMCameraFieldOfViewShaker on your camera.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Camera/Field of View")]
    [FeedbackHelp("This feedback lets you control a camera's field of view over time. You'll need a MMCameraFieldOfViewShaker on your camera.")]
    public class MMFeedbackCameraFieldOfView : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.CameraColor; } }
        /// returns the duration of the feedback
        public override float FeedbackDuration { get { return Duration; } }

        [Header("Field of View Feedback")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake, in seconds
        public float Duration = 2f;
        /// whether or not to reset shaker values after shake
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        public bool ResetTargetValuesAfterShake = true;

        [Header("Field of View")]
        /// whether or not to add to the initial value
        public bool RelativeFieldOfView = false;
        /// the curve used to animate the intensity value on
        public AnimationCurve ShakeFieldOfView = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Range(0f, 179f)]
        public float RemapFieldOfViewZero = 60f;
        /// the value to remap the curve's 1 to
        [Range(0f, 179f)]
        public float RemapFieldOfViewOne = 120f;

        /// <summary>
        /// Triggers the corresponding coroutine
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMCameraFieldOfViewShakeEvent.Trigger(ShakeFieldOfView, Duration, RemapFieldOfViewZero, RemapFieldOfViewOne, RelativeFieldOfView,
                    attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake);
            }
        }
    }
}
