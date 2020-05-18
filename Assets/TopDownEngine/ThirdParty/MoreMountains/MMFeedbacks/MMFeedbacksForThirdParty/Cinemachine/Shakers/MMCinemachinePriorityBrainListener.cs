using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this to a Cinemachine brain and it'll be able to accept custom blend transitions (used with MMFeedbackCinemachineTransition)
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachinePriorityBrainListener")]
    [RequireComponent(typeof(CinemachineBrain))]
    public class MMCinemachinePriorityBrainListener : MonoBehaviour
    {
        protected CinemachineBrain _brain;
        protected CinemachineBlendDefinition _initialDefinition;
        protected Coroutine _coroutine;

        /// <summary>
        /// On Awake we grab our brain
        /// </summary>
        protected virtual void Awake()
        {
            _brain = this.gameObject.GetComponent<CinemachineBrain>();
        }

        /// <summary>
        /// When getting an event we change our default transition if needed
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="forceMaxPriority"></param>
        /// <param name="newPriority"></param>
        /// <param name="forceTransition"></param>
        /// <param name="blendDefinition"></param>
        /// <param name="resetValuesAfterTransition"></param>
        public virtual void OnMMCinemachinePriorityEvent(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition)
        {
            if (forceTransition)
            {
                if (_coroutine != null)
                {
                    StopCoroutine(_coroutine);
                }
                else
                {
                    _initialDefinition = _brain.m_DefaultBlend;
                }
                _brain.m_DefaultBlend = blendDefinition;
                _coroutine = StartCoroutine(ResetBlendDefinition(blendDefinition.m_Time));                
            }
        }

        /// <summary>
        /// a coroutine used to reset the default transition to its initial value
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        protected virtual IEnumerator ResetBlendDefinition(float delay)
        {
            for (float timer = 0; timer < delay; timer += Time.deltaTime)
            {
                yield return null;
            }
            _brain.m_DefaultBlend = _initialDefinition;
            _coroutine = null;
        }

        /// <summary>
        /// On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            _coroutine = null;
            MMCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = null;
            MMCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
        }
    }
}
