using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Cinemachine;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback will let you change the priorities of your cameras. 
    /// It requires a bit of setup : adding a MMCinemachinePriorityListener to your different cameras, with unique Channel values on them.
    /// Optionally, you can add a MMCinemachinePriorityBrainListener on your Cinemachine Brain to handle different transition types and durations.
    /// Then all you have to do is pick a channel and a new priority on your feedback, and play it. Magic transition!
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Camera/Cinemachine Transition")]
    [FeedbackHelp("This feedback will let you change the priorities of your cameras. It requires a bit of setup : " +
        "adding a MMCinemachinePriorityListener to your different cameras, with unique Channel values on them. " +
        "Optionally, you can add a MMCinemachinePriorityBrainListener on your Cinemachine Brain to handle different transition types and durations. " +
        "Then all you have to do is pick a channel and a new priority on your feedback, and play it. Magic transition!")]
    public class MMFeedbackCinemachineTransition : MMFeedback
    {
        /// sets the inspector color for this feedback
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.CameraColor; } }
        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return Duration; } }

        [Header("Cinemachine Transition")]
        /// the channel to emit on
        public int Channel = 0;
        /// the duration of the shake in seconds
        public float Duration = 0.5f;
        /// whether or not to reset the target's values after shake
        public bool ResetValuesAfterTransition = true;

        [Header("Priority")]
        /// the new priority to apply to all virtual cameras on the specified channel
        public int NewPriority = 10;
        /// whether or not to force all virtual cameras on other channels to reset their priority to zero
        public bool ForceMaxPriority = true;
        /// whether or not to apply a new blend
        public bool ForceTransition = false;
        [MMFCondition("ForceTransition", true)]
        /// the new blend definition to apply
        public CinemachineBlendDefinition BlendDefintion;

        /// <summary>
        /// Triggers a priority change on listening virtual cameras
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active)
            {
                MMCinemachinePriorityEvent.Trigger(Channel, ForceMaxPriority, NewPriority, ForceTransition, BlendDefintion, ResetValuesAfterTransition);
            }
        }
    }
}
