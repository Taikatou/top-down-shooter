using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Rendering.PostProcessing;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Use this class to have a global PP volume auto blend on cue
    /// </summary>
    public class MMGlobalPostProcessingVolumeAutoBlend : MonoBehaviour
    {
        /// the possible blend trigger modes 
        public enum BlendTriggerModes { Start, Script }
        [Header("Blend")]
        /// the trigger mode for this MMGlobalPostProcessingVolumeAutoBlend
        public BlendTriggerModes BlendTriggerMode = BlendTriggerModes.Start;
        /// the duration of the blend (in seconds)
        public float BlendDuration = 1f;

        [Header("Weight")]
        /// the weight at the start of the blend
        [Range(0f, 1f)]
        public float InitialWeight = 0f;
        /// the desired weight at the end of the blend
        [Range(0f, 1f)]
        public float FinalWeight = 1f;

        /// Test button
        [MMFInspectorButton("Blend")]
        public bool TestBlend;
        
        protected PostProcessVolume _volume;

        /// <summary>
        /// On Awake we store our volume
        /// </summary>
        protected virtual void Awake()
        {
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.weight = InitialWeight;
        }

        /// <summary>
        /// On start we start blending if needed
        /// </summary>
        protected virtual void Start()
        {
            if (BlendTriggerMode == BlendTriggerModes.Start)
            {
                Blend();
            }
        }

        /// <summary>
        /// Blends the volume's weight from 
        /// </summary>
        public virtual void Blend()
        {
            StartCoroutine(BlendCoroutine());
        }

        protected virtual IEnumerator BlendCoroutine()
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < BlendDuration)
            {
                float timeElapsed = (Time.unscaledTime - startTime);
                float remapped = MMFeedbacksHelpers.Remap(timeElapsed, 0f, BlendDuration, InitialWeight, FinalWeight);
                _volume.weight = remapped;

                yield return null;
            }
            _volume.weight = FinalWeight;
        }
    }
}
