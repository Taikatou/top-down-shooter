using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// this feedback will let you apply forces and torques (relative or not) to a Rigidbody
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will let you apply forces and torques (relative or not) to a Rigidbody.")]
    [FeedbackPath("GameObject/Rigidbody2D")]
    public class MMFeedbackRigidbody2D : MMFeedback
    {
        public enum Modes { AddForce, AddRelativeForce, AddTorque}

        [Header("Rigidbody")]
        /// the rigidbody to target on play
        public Rigidbody2D TargetRigidbody2D;
        /// the selected mode for this feedback
        public Modes Mode = Modes.AddForce;
        /// the min force or torque to apply
        [MMFEnumCondition("Mode", (int)Modes.AddForce, (int)Modes.AddRelativeForce)]
        public Vector2 MinForce;
        /// the max force or torque to apply
        [MMFEnumCondition("Mode", (int)Modes.AddForce, (int)Modes.AddRelativeForce)]
        public Vector2 MaxForce;
        /// the min torque to apply to this rigidbody on play
        [MMFEnumCondition("Mode", (int)Modes.AddTorque)]
        public float MinTorque;
        /// the max torque to apply to this rigidbody on play
        [MMFEnumCondition("Mode", (int)Modes.AddTorque)]
        public float MaxTorque;
        /// the force mode to apply
        public ForceMode2D AppliedForceMode = ForceMode2D.Impulse;

        protected Vector2 _force;
        protected float _torque;

        /// <summary>
        /// On Custom Play, we apply our force or torque to the target rigidbody
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (TargetRigidbody2D != null))
            {
                switch (Mode)
                {
                    case Modes.AddForce:
                        _force.x = Random.Range(MinForce.x, MaxForce.x);
                        _force.y = Random.Range(MinForce.y, MaxForce.y);
                        TargetRigidbody2D.AddForce(_force, AppliedForceMode);
                        break;
                    case Modes.AddRelativeForce:
                        _force.x = Random.Range(MinForce.x, MaxForce.x);
                        _force.y = Random.Range(MinForce.y, MaxForce.y);
                        TargetRigidbody2D.AddRelativeForce(_force, AppliedForceMode);
                        break;
                    case Modes.AddTorque:
                        _torque = Random.Range(MinTorque, MaxTorque);
                        TargetRigidbody2D.AddTorque(_torque, AppliedForceMode);
                        break;
                }
            }
        }
    }
}
