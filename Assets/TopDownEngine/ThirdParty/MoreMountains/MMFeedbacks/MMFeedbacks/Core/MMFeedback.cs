using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A base class, meant to be extended, defining a Feedback. A Feedback is an action triggered by a MMFeedbacks, usually in reaction to the player's input or actions,
    /// to help communicate both emotion and legibility, improving game feel.
    /// To create a new feedback, extend this class and override its Custom methods, declared at the end of this class. You can look at the many examples for reference.
    /// </summary>
    [AddComponentMenu("")]
    [System.Serializable]
    public abstract class MMFeedback : MonoBehaviour
    {
        /// whether or not this feedback is active
        public bool Active = true;        
        /// the name of this feedback to display in the inspector
        public string Label = "MMFeedback";
        /// the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)
        [Range(0,100)]
        public float Chance = 100f;
        /// a number of timing-related values (delay, repeat, etc)
        public MMFeedbackTiming Timing; 
        /// the Owner of the feedback, as defined when calling the Initialization method
        public GameObject Owner { get; set; }
        [HideInInspector]
        /// whether or not this feedback is in debug mode
        public bool DebugActive = false;
        /// set this to true if your feedback should pause the execution of the feedback sequence
        public virtual YieldInstruction Pause { get { return null; } }
        /// if this is true, this feedback will wait until all previous feedbacks have run
        public virtual bool HoldingPause { get { return false; } }
        /// if this is true, this feedback will wait until all previous feedbacks have run, then run all previous feedbacks again
        public virtual bool LooperPause { get { return false; } }
        /// if this is true, this feedback will wait until all previous feedbacks have run, then run all previous feedbacks again
        public virtual bool LooperStart { get { return false; } }
        /// an overridable color for your feedback, that can be redefined per feedback. White is the only reserved color, and the feedback will revert to 
        /// normal (light or dark skin) when left to White
        public virtual Color FeedbackColor { get { return Color.white;  } }
        
        /// the time (or unscaled time) based on the selected Timing settings
        public float FeedbackTime { get {
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.time;
                }
                else
                {
                    return Time.unscaledTime;
                }
        } }
        /// the delta time (or unscaled delta time) based on the selected Timing settings
        public float FeedbackDeltaTime
        {
            get
            {
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.deltaTime;
                }
                else
                {
                    return Time.unscaledDeltaTime;
                }
            }
        }


        // the timestamp at which this feedback was last played
        public virtual float FeedbackStartedAt { get { return _lastPlayTimestamp; } }
        // the perceived duration of the feedback, to be used to display its progress bar, meant to be overridden with meaningful data by each feedback
        public virtual float FeedbackDuration { get { return 0f; } }
        /// whether or not this feedback is playing right now
        public virtual bool FeedbackPlaying { get { return ((FeedbackStartedAt > 0f) && (Time.time - FeedbackStartedAt < FeedbackDuration)); } }

        protected WaitForSeconds _initialDelayWaitForSeconds;
        protected WaitForSeconds _betweenDelayWaitForSeconds;
        protected WaitForSeconds _sequenceDelayWaitForSeconds;
        protected float _lastPlayTimestamp = -1f;
        protected int _playsLeft;
        protected bool _initialized = false;
        protected Coroutine _playCoroutine;
        protected Coroutine _infinitePlayCoroutine;
        protected Coroutine _sequenceCoroutine;
        protected Coroutine _repeatedPlayCoroutine;
        protected int _sequenceTrackID = 0;

        /// <summary>
        /// Initializes the feedback and its timing related variables
        /// </summary>
        /// <param name="owner"></param>
        public virtual void Initialization(GameObject owner)
        {
            _initialized = true;
            Owner = owner;
            _playsLeft = Timing.NumberOfRepeats + 1;

            if (Timing.InitialDelay > 0f)
            {
                _initialDelayWaitForSeconds = new WaitForSeconds(Timing.InitialDelay);
            }

            if (Timing.DelayBetweenRepeats > 0f)
            {
                _betweenDelayWaitForSeconds = new WaitForSeconds(Timing.DelayBetweenRepeats + FeedbackDuration);
            }
            
            if (Timing.Sequence != null)
            {
                _sequenceDelayWaitForSeconds = new WaitForSeconds(Timing.DelayBetweenRepeats + Timing.Sequence.Length);

                for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                {
                    if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
                    {
                        _sequenceTrackID = i;
                    }
                }
            }

            CustomInitialization(owner);            
        }

        /// <summary>
        /// Plays the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void Play(Vector3 position, float attenuation = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            if (!_initialized)
            {
                Debug.LogWarning("The " + this + " feedback is being played without having been initialized. Call Initialization() first.");
            }
            
            // we check the cooldown
            if ((Timing.CooldownDuration > 0f) && (FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration))
            {
                return;
            }

            if (Timing.InitialDelay > 0f) 
            {
                _playCoroutine = StartCoroutine(PlayCoroutine(position, attenuation));
            }
            else
            {
                _lastPlayTimestamp = FeedbackTime;
                RegularPlay(position, attenuation);
            }  
        }
        
        /// <summary>
        /// An internal coroutine delaying the initial play of the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator PlayCoroutine(Vector3 position, float attenuation = 1.0f)
        {
            yield return _initialDelayWaitForSeconds;
            _lastPlayTimestamp = FeedbackTime;
            RegularPlay(position, attenuation);
        }

        /// <summary>
        /// Triggers delaying coroutines if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected virtual void RegularPlay(Vector3 position, float attenuation = 1.0f)
        {
            if (Chance == 0f)
            {
                return;
            }
            if (Chance != 100f)
            {
                // determine the odds
                float random = Random.Range(0f, 100f);
                if (random > Chance)
                {
                    return;
                }
            }

            if (Timing.RepeatForever)
            {
                _infinitePlayCoroutine = StartCoroutine(InfinitePlay(position, attenuation));
                return;
            }
            if (Timing.NumberOfRepeats > 0)
            {
                _repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(position, attenuation));
                return;
            }            
            if (Timing.Sequence == null)
            {
                CustomPlayFeedback(position, attenuation);
            }
            else
            {
                _sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, attenuation));
            }
            
        }

        /// <summary>
        /// Internal coroutine used for repeated play without end
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator InfinitePlay(Vector3 position, float attenuation = 1.0f)
        {
            while (true)
            {
                _lastPlayTimestamp = FeedbackTime;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, attenuation);
                    yield return _betweenDelayWaitForSeconds;
                }
                else
                {
                    _sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, attenuation));
                    yield return _sequenceDelayWaitForSeconds;
                }
            }
        }

        /// <summary>
        /// Internal coroutine used for repeated play
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        /// <returns></returns>
        protected virtual IEnumerator RepeatedPlay(Vector3 position, float attenuation = 1.0f)
        {
            while (_playsLeft > 0)
            {
                _lastPlayTimestamp = FeedbackTime;
                _playsLeft--;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, attenuation);
                    yield return _betweenDelayWaitForSeconds;
                }
                else
                {
                    _sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, attenuation));
                    yield return _sequenceDelayWaitForSeconds;
                }
            }
            _playsLeft = Timing.NumberOfRepeats + 1;
        }

        protected float _beatInterval;
        protected bool BeatThisFrame = false;
        protected int LastBeatIndex = 0;
        protected int CurrentSequenceIndex = 0;
        protected float LastBeatTimestamp = 0f;

        protected virtual IEnumerator SequenceCoroutine(Vector3 position, float attenuation = 1.0f)
        {
            yield return null;
            float timeStartedAt = FeedbackTime;
            float lastFrame = FeedbackTime;

            BeatThisFrame = false;
            LastBeatIndex = 0;
            CurrentSequenceIndex = 0;
            LastBeatTimestamp = 0f;

            if (Timing.Quantized)
            {
                while (CurrentSequenceIndex < Timing.Sequence.QuantizedSequence[0].Line.Count)
                {
                    _beatInterval = 60f / Timing.TargetBPM;

                    if ((FeedbackTime - LastBeatTimestamp >= _beatInterval) || (LastBeatTimestamp == 0f))
                    {
                        BeatThisFrame = true;
                        LastBeatIndex = CurrentSequenceIndex;
                        LastBeatTimestamp = FeedbackTime;

                        for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                        {
                            if (Timing.Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID == Timing.TrackID)
                            {
                                CustomPlayFeedback(position, attenuation);
                            }
                        }
                        CurrentSequenceIndex++;
                    }
                    yield return null;
                }
            }
            else
            {
                while (FeedbackTime - timeStartedAt < Timing.Sequence.Length)
                {
                    foreach (MMSequenceNote item in Timing.Sequence.OriginalSequence.Line)
                    {
                        if ((item.ID == Timing.TrackID) && (item.Timestamp >= lastFrame) && (item.Timestamp <= FeedbackTime - timeStartedAt))
                        {
                            CustomPlayFeedback(position, attenuation);
                        }
                    }
                    lastFrame = FeedbackTime - timeStartedAt;
                    yield return null;
                }
            }
                    
        }

        /// <summary>
        /// Stops all feedbacks from playing. Will stop repeating feedbacks, and call custom stop implementations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void Stop(Vector3 position, float attenuation = 1.0f)
        {
            if (_playCoroutine != null) { StopCoroutine(_playCoroutine); }
            if (_infinitePlayCoroutine != null) { StopCoroutine(_infinitePlayCoroutine); }
            if (_repeatedPlayCoroutine != null) { StopCoroutine(_repeatedPlayCoroutine); }            
            if (_sequenceCoroutine != null) { StopCoroutine(_sequenceCoroutine);  }

            _lastPlayTimestamp = 0f;
            _playsLeft = Timing.NumberOfRepeats + 1;
            CustomStopFeedback(position, attenuation);
        }

        public virtual void ResetFeedback()
        {
            _playsLeft = Timing.NumberOfRepeats + 1;
            CustomReset();
        }
        
        /// <summary>
        /// This method describes all custom initialization processes the feedback requires, in addition to the main Initialization method
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(GameObject owner) { }

        /// <summary>
        /// This method describes what happens when the feedback gets played
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f);

        /// <summary>
        /// This method describes what happens when the feedback gets stopped
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float attenuation = 1.0f) { }

        /// <summary>
        /// This method describes what happens when the feedback gets reset
        /// </summary>
        protected virtual void CustomReset() { }
    }   
}

