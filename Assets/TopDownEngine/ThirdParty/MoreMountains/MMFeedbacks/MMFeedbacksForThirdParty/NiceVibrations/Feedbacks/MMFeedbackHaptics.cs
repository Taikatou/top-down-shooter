using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

#if MOREMOUNTAINS_NICEVIBRATIONS
using MoreMountains.NiceVibrations;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this feedback to be able to trigger haptic feedbacks via the NiceVibration library.
    /// It'll let you create transient or continuous vibrations, play presets or advanced patterns via AHAP files, and stop any vibration at any time
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Haptics")]
    [FeedbackHelp("This feedback lets you trigger haptic feedbacks through the Nice Vibrations asset, available on the Unity Asset Store. You'll need to own that asset and have it " +
        "in your project for this to work.")]
    public class MMFeedbackHaptics : MMFeedback
    {
        /// the possible haptic methods for this feedback
        public enum HapticMethods { NativePreset, Transient, Continuous, AdvancedPattern, Stop }
        /// the timescale to operate on
        public enum Timescales { ScaledTime, UnscaledTime }

        [Header("Haptics")]
        public HapticMethods HapticMethod = HapticMethods.NativePreset;

        [MMFEnumCondition("HapticMethod", (int)HapticMethods.NativePreset)]
        public HapticTypes HapticType = HapticTypes.None; 

        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Transient)]
        public float TransientIntensity = 1f;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Transient)]
        public float TransientSharpness = 1f;

        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
        public float InitialContinuousIntensity = 1f;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
        public AnimationCurve ContinuousIntensityCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1f, 1f));
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
        public float InitialContinuousSharpness = 1f;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
        public AnimationCurve ContinuousSharpnessCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1f, 1f));
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
        public float ContinuousDuration = 1f;

        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public TextAsset AHAPFileForIOS;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public MMNVAndroidWaveFormAsset AndroidWaveFormFile;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public int AndroidRepeat = -1;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public HapticTypes OldIOSFallback;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public Timescales Timescale = Timescales.UnscaledTime;


        protected static bool _continuousPlaying = false;
        protected static float _continuousStartedAt = 0f;

        /// <summary>
        /// When this feedback gets played
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            switch(HapticMethod)
            {
                case HapticMethods.AdvancedPattern:
                    MMVibrationManager.AdvancedHapticPattern(AHAPFileForIOS.text, AndroidWaveFormFile.WaveForm.Pattern, AndroidWaveFormFile.WaveForm.Amplitudes, AndroidRepeat, OldIOSFallback);
                    break;

                case HapticMethods.Continuous:
                    StartCoroutine(ContinuousHapticsCoroutine());
                    break;

                case HapticMethods.NativePreset:
                    MMVibrationManager.Haptic(HapticType);
                    break;

                case HapticMethods.Transient:
                    MMVibrationManager.TransientHaptic(TransientIntensity, TransientSharpness);
                    break;

                case HapticMethods.Stop:
                    if (_continuousPlaying)
                    {
                        MMVibrationManager.StopContinuousHaptic();
                        _continuousPlaying = false;
                    }                    
                    break;
            }
        }

        /// <summary>
        /// A coroutine used to update continuous haptics as they're playing
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator ContinuousHapticsCoroutine()
        {
            _continuousStartedAt = (Timescale == Timescales.ScaledTime) ? Time.time : Time.unscaledTime;
            _continuousPlaying = true;
            float elapsedTime = ComputeElapsedTime();

            MMVibrationManager.ContinuousHaptic(InitialContinuousIntensity, InitialContinuousSharpness, ContinuousDuration, HapticTypes.Success);

            while (_continuousPlaying && (elapsedTime < ContinuousDuration))
            {
                elapsedTime = ComputeElapsedTime();
                float remappedTime = MMFeedbacksHelpers.Remap(elapsedTime, 0f, ContinuousDuration, 0f, 1f);
                float intensity = ContinuousIntensityCurve.Evaluate(remappedTime);
                float sharpness = ContinuousSharpnessCurve.Evaluate(remappedTime);
                MMNViOSCoreHaptics.UpdateContinuousHapticPatternRational(intensity, sharpness);
                yield return null;
            }
            if (_continuousPlaying)
            {
                _continuousPlaying = false;
                MMVibrationManager.StopContinuousHaptic();
            }            
        }

        /// <summary>
        /// This methods computes and returns the elapsed time since the start of the last played continuous haptic
        /// </summary>
        /// <returns></returns>
        protected virtual float ComputeElapsedTime()
        {
            float elapsedTime = (Timescale == Timescales.ScaledTime) ? Time.time - _continuousStartedAt : Time.unscaledTime - _continuousStartedAt;
            return elapsedTime;
        }
    }
}
#endif