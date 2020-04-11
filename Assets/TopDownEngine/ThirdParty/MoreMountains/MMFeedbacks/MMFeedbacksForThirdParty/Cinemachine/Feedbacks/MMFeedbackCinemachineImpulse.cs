using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Cinemachine;

namespace MoreMountains.FeedbacksForThirdParty
{
    [AddComponentMenu("")]
    [FeedbackPath("Camera/Cinemachine Impulse")]
    [FeedbackHelp("This feedback lets you trigger a Cinemachine Impulse event. You'll need a Cinemachine Impulse Listener on your camera for this to work.")]
    public class MMFeedbackCinemachineImpulse : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.CameraColor; } }

        [Header("Cinemachine Impulse")]
        [CinemachineImpulseDefinitionProperty]
        public CinemachineImpulseDefinition m_ImpulseDefinition;
        public Vector3 Velocity;


        /// the duration of this feedback is the duration of the impulse
        public override float FeedbackDuration { get { return m_ImpulseDefinition.m_TimeEnvelope.Duration; } }

        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                m_ImpulseDefinition.CreateEvent(position, Velocity);
            }
        }
    }
}
