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
        [MMNVEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public MMNVRumbleWaveFormAsset RumbleWaveFormFile;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public int AndroidRepeat = -1;
        [MMNVEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public int RumbleRepeat = -1;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public HapticTypes OldIOSFallback;
        [MMFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
        public Timescales Timescale = Timescales.UnscaledTime;

        [Header("Rumble")]
        public bool AllowRumble = true;

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

                    string iOSString = (AHAPFileForIOS == null) ? "" : AHAPFileForIOS.text;

                    long[] androidPattern = (AndroidWaveFormFile == null) ? null : AndroidWaveFormFile.WaveForm.Pattern;
                    int[] androidAmplitude = (AndroidWaveFormFile == null) ? null : AndroidWaveFormFile.WaveForm.Amplitudes;

                    long[] rumblePattern = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.Pattern;
                    int[] lowFreqAmplitude = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.LowFrequencyAmplitudes;
                    int[] highFreqAmplitude = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.HighFrequencyAmplitudes;

                    MMVibrationManager.AdvancedHapticPattern(iOSString, androidPattern, androidAmplitude, AndroidRepeat,
                                                                        rumblePattern, lowFreqAmplitude, highFreqAmplitude, RumbleRepeat,
                                                                OldIOSFallback, this);
                    break;

                case HapticMethods.Continuous:
                    StartCoroutine(ContinuousHapticsCoroutine());
                    break;

                case HapticMethods.NativePreset:
                    MMVibrationManager.Haptic(HapticType, false, AllowRumble, this);
                    break;

                case HapticMethods.Transient:
                    MMVibrationManager.TransientHaptic(TransientIntensity, TransientSharpness, AllowRumble, this);
                    break;

                case HapticMethods.Stop:
                    if (_continuousPlaying)
                    {
                        MMVibrationManager.StopContinuousHaptic(AllowRumble);
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

            MMVibrationManager.ContinuousHaptic(InitialContinuousIntensity, InitialContinuousSharpness, ContinuousDuration, HapticTypes.Success, this);

            while (_continuousPlaying && (elapsedTime < ContinuousDuration))
            {
                elapsedTime = ComputeElapsedTime();
                float remappedTime = Remap(elapsedTime, 0f, ContinuousDuration, 0f, 1f);
                float intensity = ContinuousIntensityCurve.Evaluate(remappedTime);
                float sharpness = ContinuousSharpnessCurve.Evaluate(remappedTime);
                MMVibrationManager.UpdateContinuousHaptic(intensity, sharpness, true);
                if (AllowRumble)
                {
                    #if MOREMOUNTAINS_NICEVIBRATIONS_RUMBLE
                        MMNVRumble.RumbleContinuous(intensity, sharpness);
                    #endif
                }
                yield return null;
            }
            if (_continuousPlaying)
            {
                _continuousPlaying = false;
                MMVibrationManager.StopContinuousHaptic(AllowRumble);
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

        /// <summary>
        /// Remaps value x from AB to CD
        /// </summary>
        /// <param name="x"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            float remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}
#endif