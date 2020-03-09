using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// the possible modes for the timescale
    public enum TimescaleModes { Scaled, Unscaled }

    /// <summary>
    /// A class collecting delay, cooldown and repeat values, to be used to define the behaviour of each MMFeedback
    /// </summary>
    [System.Serializable]
    public class MMFeedbackTiming
    {
        [Header("Timescale")]
        /// whether we're working on scaled or unscaled time
        public TimescaleModes TimescaleMode = TimescaleModes.Scaled;

        [Header("Delays")]
        /// the initial delay to apply before playing the delay (in seconds)
        public float InitialDelay = 0f;
        /// the cooldown duration mandatory between two plays
        public float CooldownDuration = 0f;

        [Header("Repeat")]
        /// the repeat mode, whether the feedback should be played once, multiple times, or forever
        public int NumberOfRepeats = 0;
        /// if this is true, the feedback will be repeated forever
        public bool RepeatForever = false;
        /// the delay (in seconds) between repeats
        public float DelayBetweenRepeats = 1f;

        [Header("Sequence")]
        public MMSequence Sequence;
        public int TrackID = 0;
        public bool Quantized = false;
        [MMFCondition("Quantized", true)]
        public int TargetBPM = 120;
    }
}
