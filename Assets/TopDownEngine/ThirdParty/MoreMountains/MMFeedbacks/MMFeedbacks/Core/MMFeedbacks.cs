using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// A collection of MMFeedback, meant to be played altogether.
    /// This class provides a custom inspector to add and customize feedbacks, and public methods to trigger them, stop them, etc.
    /// You can either use it on its own, or bind it from another class and trigger it from there.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("More Mountains/Feedbacks/MMFeedbacks")]
    [DisallowMultipleComponent]
    public class MMFeedbacks : MonoBehaviour
    {
        /// a list of MMFeedback to trigger
        public List<MMFeedback> Feedbacks = new List<MMFeedback>();
        /// the possible initialization modes. If you use Script, you'll have to initialize manually by calling the Initialization method and passing it an owner
        /// Otherwise, you can have this component initialize itself at Awake or Start, and in this case the owner will be the MMFeedbacks itself
        public enum InitializationModes { Script, Awake, Start }
        /// the chosen initialization mode
        public InitializationModes InitializationMode = InitializationModes.Start;
        /// whether or not to play this feedbacks automatically on Start
        public bool AutoPlayOnStart = false;
        [HideInInspector]
        /// whether or not this MMFeedbacks is in debug mode
        public bool DebugActive = false;
        /// whether or not this MMFeedbacks is playing right now - meaning it hasn't been stopped yet.
        /// if you don't stop your MMFeedbacks it'll remain true of course
        public bool IsPlaying { get; protected set; }

        protected float _startTime = 0f;
        protected float _holdingMax = 0f;
        protected float _lastStartAt = 0f;

        /// <summary>
        /// On Awake we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected virtual void Awake()
        {
            if ((InitializationMode == InitializationModes.Awake) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// On Start we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected virtual void Start()
        {
            if ((InitializationMode == InitializationModes.Start) && (Application.isPlaying))
            {
                Initialization(this.gameObject);
            }
            if (AutoPlayOnStart && Application.isPlaying)
            {
                PlayFeedbacks();
            }
        }

        /// <summary>
        /// Initializes the MMFeedbacks, setting this MMFeedbacks as the owner
        /// </summary>
        public virtual void Initialization()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {

                Feedbacks[i].Initialization(this.gameObject);
            }
        }

        /// <summary>
        /// A public method to initialize the feedback, specifying an owner that will be used as the reference for position and hierarchy by feedbacks
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="feedbacksOwner"></param>
        public virtual void Initialization(GameObject owner)
        {
            IsPlaying = false;
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                if (Feedbacks[i] != null)
                {
                    Feedbacks[i].Initialization(owner);
                }                
            }
        }

        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation
        /// </summary>
        public virtual void PlayFeedbacks()
        {
            PlayFeedbacksInternal(this.transform.position, 1.0f);
        }

        /// <summary>
        /// Plays all feedbacks, specifying a position and attenuation. The position may be used by each Feedback and taken into account to spark a particle or play a sound for example.
        /// The attenuation is a factor that can be used by each Feedback to lower its intensity, usually you'll want to define that attenuation based on time or distance (using a lower 
        /// attenuation value for feedbacks happening further away from the Player).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksOwner"></param>
        /// <param name="attenuation"></param>
        public virtual void PlayFeedbacks(Vector3 position, float attenuation = 1.0f)
        {
            PlayFeedbacksInternal(position, attenuation);
        }

        /// <summary>
        /// An internal method used to play feedbacks, shouldn't be called externally
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected virtual void PlayFeedbacksInternal(Vector3 position, float attenuation)
        {
            _startTime = Time.time;
            _holdingMax = 0f;
            _lastStartAt = _startTime;

            ResetFeedbacks();

            // test if a pause or holding pause is found
            bool pauseFound = false;
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                if ((Feedbacks[i].Pause != null) && (Feedbacks[i].Active))
                {
                    pauseFound = true;
                }
                if ((Feedbacks[i].HoldingPause == true) && (Feedbacks[i].Active))
                {
                    pauseFound = true;
                }
            }

            if (!pauseFound)
            {
                // if no pause was found, we just play all feedbacks at once
                IsPlaying = true;
                for (int i = 0; i < Feedbacks.Count; i++)
                {
                    Feedbacks[i].Play(position, attenuation);
                }
            }
            else
            {
                // if at least one pause was found
                StartCoroutine(PausedFeedbacksCo(position, attenuation));
            }            
        }

        protected virtual IEnumerator PausedFeedbacksCo(Vector3 position, float attenuation)
        {
            IsPlaying = true;
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                // handles holding pauses
                if ( (Feedbacks[i].Active) 
                    && ((Feedbacks[i].HoldingPause == true) || (Feedbacks[i].LooperPause == true)))
                {
                    // we stay here until all previous feedbacks have finished
                    while (Time.time - _lastStartAt < _holdingMax)
                    {
                        yield return null;
                    }

                    _holdingMax = 0f;
                    _lastStartAt = Time.time;
                }

                // plays the feedback
                Feedbacks[i].Play(position, attenuation);

                // Handles pause
                if ((Feedbacks[i].Pause != null) 
                    && (Feedbacks[i].Active))
                {
                    bool shouldPause = true;
                    if (Feedbacks[i].Chance < 100)
                    {
                        float random = Random.Range(0f, 100f);
                        if (random > Feedbacks[i].Chance)
                        {
                            shouldPause = false;
                        }
                    }

                    if (shouldPause)
                    {
                        yield return Feedbacks[i].Pause;
                        _lastStartAt = Time.time;
                        _holdingMax = 0f;
                    }                    
                }

                // updates holding max
                if (Feedbacks[i].Active)
                {                    
                    if (Feedbacks[i].Pause == null)
                    {
                        float feedbackDuration = Feedbacks[i].FeedbackDuration + Feedbacks[i].Timing.InitialDelay + Feedbacks[i].Timing.NumberOfRepeats * (Feedbacks[i].FeedbackDuration + Feedbacks[i].Timing.DelayBetweenRepeats);
                        _holdingMax = Mathf.Max(feedbackDuration, _holdingMax);
                    }                        
                }

                // handles looper
                if ( (Feedbacks[i].LooperPause == true) 
                    && (Feedbacks[i].Active) 
                    && ((Feedbacks[i] as MMFeedbackLooper).NumberOfLoopsLeft > 0) )
                {
                    // we determine the index we should start again at
                    bool loopAtLastPause = (Feedbacks[i] as MMFeedbackLooper).LoopAtLastPause;
                    bool loopAtLastLoopStart = (Feedbacks[i] as MMFeedbackLooper).LoopAtLastLoopStart;
                    int newi = 0;                    
                    for (int j = i - 1; j >= 0; j--)
                    {
                        // if we're at the start
                        if (j == 0)
                        {
                            newi = j-1;
                            break;
                        }
                        // if we've found a pause
                        if ( (Feedbacks[j].Pause != null) 
                            && (Feedbacks[j].FeedbackDuration > 0f) 
                            && loopAtLastPause && (Feedbacks[j].Active) )
                        {
                            newi = j;
                            break;
                        }
                        // if we've found a looper start
                        if ( (Feedbacks[j].LooperStart == true) 
                            && loopAtLastLoopStart 
                            && (Feedbacks[j].Active) )
                        {
                            newi = j;
                            break;
                        }
                    }
                    i = newi;
                }
            }
        }

        /// <summary>
        /// Stops all feedbacks from playing. 
        /// </summary>
        public virtual void StopFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Stop(this.transform.position, 1.0f);
            }
            IsPlaying = false;
        }

        /// <summary>
        /// Stops all feedbacks from playing, specifying a position and attenuation that can be used by the Feedbacks 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        public virtual void StopFeedbacks(Vector3 position, float attenuation = 1.0f)
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].Stop(position, attenuation);
            }
            IsPlaying = false;
        }

        /// <summary>
        /// Calls each feedback's Reset method if they've defined one. An example of that can be resetting the initial color of a flickering renderer.
        /// </summary>
        public virtual void ResetFeedbacks()
        {
            for (int i = 0; i < Feedbacks.Count; i++)
            {
                Feedbacks[i].ResetFeedback();
            }
            IsPlaying = false;
        }

        /// <summary>
        /// On Disable we stop all feedbacks
        /// </summary>
        protected virtual void OnDisable()
        {
            /*if (IsPlaying)
            {
                StopFeedbacks();
                StopAllCoroutines();
            }*/
        }

        /// <summary>
        /// On Destroy, removes all feedbacks from this MMFeedbacks to avoid any leftovers
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsPlaying = false;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {            
                // we remove all binders
                foreach (MMFeedback feedback in Feedbacks)
                {
                    EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(feedback);
                    };                    
                }
            }
            #endif
        }
            
    }

}
