﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will send a shake event when played
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("Define camera shake properties (duration in seconds, amplitude and frequency), and this will broadcast a MMCameraShakeEvent with these same settings. You'll need to add a MMCinemachineCameraShaker on your camera for this to work (or a MMCinemachineZoom component if you're using Cinemachine). Note that although this event and system was built for cameras in mind, you could technically use it to shake other objects as well.")]
    [FeedbackPath("Camera/Camera Shake")]
    public class MMFeedbackCameraShake : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.CameraColor; } }

        [Header("Camera Shake")]
        /// whether or not this shake should repeat forever, until stopped
        public bool RepeatUntilStopped = false;
        /// the channel to broadcast this shake on
        public int Channel = 0;
        /// the properties of the shake (duration, intensity, frequenc)
        public MMCameraShakeProperties CameraShakeProperties = new MMCameraShakeProperties(0.1f, 0.2f, 40f);
        
        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return CameraShakeProperties.Duration; } }

        /// <summary>
        /// On Play, sends a shake camera event
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMCameraShakeEvent.Trigger(CameraShakeProperties.Duration, CameraShakeProperties.Amplitude * attenuation, CameraShakeProperties.Frequency, RepeatUntilStopped, Channel);
            }
        }

        protected override void CustomStopFeedback(Vector3 position, float attenuation = 1)
        {
            base.CustomStopFeedback(position, attenuation);
            if (Active)
            {
                MMCameraShakeStopEvent.Trigger(Channel);
            }
        }
    }
}
