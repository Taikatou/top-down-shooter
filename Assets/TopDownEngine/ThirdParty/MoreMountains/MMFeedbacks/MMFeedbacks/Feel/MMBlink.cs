using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Describes a blink phase, defined by a duration for the phase, and the time it should remain inactive and active, sequentially
    /// For the duration of the phase, the object will be off for OffDuration, then on for OnDuration, then off again for OffDuration, etc
    /// If you want a grenade to blink briefly every .2 seconds, for 1 second, these parameters are what you're after :
    /// PhaseDuration = 1f;
    /// OffDuration = 0.2f;
    /// OnDuration = 0.1f;
    /// </summary>
    [Serializable]
    public class BlinkPhase
    {
        /// the duration of that specific phase, in seconds
        public float PhaseDuration = 1f;
        /// the time the object should remain off
        public float OffDuration = 0.2f;
        /// the time the object should then remain on
        public float OnDuration = 0.1f;
        /// the speed at which to lerp to off state
        public float OffLerpDuration = 0.05f;
        /// the speed at which to lerp to on state
        public float OnLerpDuration = 0.05f;
    }

    /// <summary>
    /// Add this class to a GameObject to make it blink, either by enabling/disabling a gameobject, changing its alpha, emission intensity, or a value on a shader)
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Various/MMBlink")]
    public class MMBlink : MonoBehaviour
    {
        /// the possible methods to blink an object
        public enum Methods { SetGameObjectActive, MaterialAlpha, MaterialEmissionIntensity, ShaderFloatValue }
        /// the selected method to blink the target object
        [Header("Blink Method")]
        public Methods Method = Methods.SetGameObjectActive;
        /// the object to set active/inactive if that method was chosen
        [MMFEnumCondition("Method", (int)Methods.SetGameObjectActive)]
        public GameObject TargetGameObject;
        /// the target renderer to work with
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity, (int)Methods.ShaderFloatValue)]
        public Renderer TargetRenderer;
        /// the shader property to alter a float on
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.ShaderFloatValue)]
        public string ShaderPropertyName = "";
        /// the value to apply when blinking is off
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity, (int)Methods.ShaderFloatValue)]
        public float OffValue = 0f;
        /// the value to apply when blinking is on
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity, (int)Methods.ShaderFloatValue)]
        public float OnValue = 1f;
        /// whether to lerp these values or not
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity, (int)Methods.ShaderFloatValue)]
        public bool LerpValue = true;
        /// the curve to apply to the lerping
        [MMFEnumCondition("Method", (int)Methods.MaterialAlpha, (int)Methods.MaterialEmissionIntensity, (int)Methods.ShaderFloatValue)]
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1.05f), new Keyframe(1, 0));

        [Header("State")]
        /// whether the object should blink or not
        public bool Blinking = true;


        [Header("Sequence")]
        /// how many times the sequence should repeat (-1 : infinite)
        public int RepeatCount = 0;
        /// The list of phases to apply blinking with
        public List<BlinkPhase> Phases;

        [Header("Debug")]
        /// Test button
        [MMFInspectorButton("ToggleBlinking")]
        public bool ToggleBlinkingButton;
        /// Test button
        [MMFInspectorButton("StartBlinking")]
        public bool StartBlinkingButton;
        /// Test button
        [MMFInspectorButton("StopBlinking")]
        public bool StopBlinkingButton;
        /// is the blinking object in an active state right now?
        [MMFReadOnly]
        public bool Active = false;
        /// the index of the phase we're currently in
        [MMFReadOnly]
        public int CurrentPhaseIndex = 0;

        protected float _lastBlinkAt = 0f;
        protected float _currentPhaseStartedAt = 0f;
        protected float _currentBlinkDuration;
        protected float _currentLerpDuration;
        protected int _propertyID;
        protected float _initialShaderFloatValue;
        protected Color _initialColor;
        protected Color _currentColor;
        protected int _repeatCount;

        /// <summary>
        /// Makes the object blink if it wasn't already blinking, stops it otherwise
        /// </summary>
        public virtual void ToggleBlinking()
        {
            Blinking = !Blinking;
            ResetBlinkProperties();
        }

        /// <summary>
        /// Makes the object start blinking
        /// </summary>
        public virtual void StartBlinking()
        {
            Blinking = true;
            ResetBlinkProperties();
        }

        /// <summary>
        /// Makes the object stop blinking
        /// </summary>
        public virtual void StopBlinking()
        {
            Blinking = false;
            ResetBlinkProperties();
        }
                
        /// <summary>
        /// On Update, we blink if we are supposed to
        /// </summary>
        protected virtual void Update()
        {
            DetermineState();

            if (!Blinking)
            {
                return;
            }

            Blink();
        }

        /// <summary>
        /// Determines the current phase and determines whether the object should be active or inactive
        /// </summary>
        protected virtual void DetermineState()
        {
            DetermineCurrentPhase();
            
            if (!Blinking)
            {
                return;
            }

            if (Active)
            {
                if (Time.time - _lastBlinkAt > Phases[CurrentPhaseIndex].OnDuration)
                {
                    Active = false;
                    _lastBlinkAt = Time.time;
                }
            }
            else
            {
                if (Time.time - _lastBlinkAt > Phases[CurrentPhaseIndex].OffDuration)
                {
                    Active = true;
                    _lastBlinkAt = Time.time;
                }
            }
            _currentBlinkDuration = Active ? Phases[CurrentPhaseIndex].OnDuration : Phases[CurrentPhaseIndex].OffDuration;
            _currentLerpDuration = Active ? Phases[CurrentPhaseIndex].OnLerpDuration : Phases[CurrentPhaseIndex].OffLerpDuration;
        }

        /// <summary>
        /// Blinks the object based on its computed state
        /// </summary>
        protected virtual void Blink()
        {
            float currentValue = _currentColor.a;
            float initialValue = Active ? OffValue : OnValue;
            float targetValue = Active ? OnValue : OffValue;
            float newValue = targetValue;

            if (LerpValue && (Time.time - _lastBlinkAt < _currentLerpDuration))
            {
                float t = MMFeedbacksHelpers.Remap(Time.time - _lastBlinkAt, 0f, _currentLerpDuration, 0f, 1f);
                newValue = Curve.Evaluate(t);
                newValue = MMFeedbacksHelpers.Remap(newValue, 0f, 1f, initialValue, targetValue);
            }
            else
            {
                newValue = targetValue;
            }

            switch (Method)
            {
                case Methods.SetGameObjectActive:
                    TargetGameObject.SetActive(Active);
                    break;
                case Methods.MaterialAlpha:
                    _currentColor.a = newValue;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
                case Methods.MaterialEmissionIntensity:
                    _currentColor = _initialColor * newValue;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
                case Methods.ShaderFloatValue:
                    TargetRenderer.material.SetFloat(_propertyID, newValue);
                    break;
            }
        }

        /// <summary>
        /// Determines the current phase index based on phase durations
        /// </summary>
        protected virtual void DetermineCurrentPhase()
        {
            // if the phase duration is null or less, we'll be in that phase forever, and return
            if (Phases[CurrentPhaseIndex].PhaseDuration <= 0)
            {
                return;
            }
            // if the phase's duration is elapsed, we move to the next phase
            if (Time.time - _currentPhaseStartedAt > Phases[CurrentPhaseIndex].PhaseDuration)
            {
                CurrentPhaseIndex++;
                _currentPhaseStartedAt = Time.time;
            }
            if (CurrentPhaseIndex > Phases.Count -1)
            {
                CurrentPhaseIndex = 0;
                if (RepeatCount != -1)
                {
                    _repeatCount--;
                    if (_repeatCount < 0)
                    {
                        ResetBlinkProperties();
                        Blinking = false;
                    }
                }                
            }
        }
        
        /// <summary>
        /// On enable, initializes blink properties
        /// </summary>
        protected virtual void OnEnable()
        {
            InitializeBlinkProperties();            
        }

        /// <summary>
        /// Resets counters and grabs properties and initial colors
        /// </summary>
        protected virtual void InitializeBlinkProperties()
        {
            _currentPhaseStartedAt = Time.time;
            CurrentPhaseIndex = 0;
            _repeatCount = RepeatCount;

            switch (Method)
            {
                case Methods.MaterialAlpha:
                    _propertyID = Shader.PropertyToID("_Color");
                    _initialColor = TargetRenderer.material.GetColor(_propertyID);
                    _currentColor = _initialColor;
                    break;
                case Methods.MaterialEmissionIntensity:
                    _propertyID = Shader.PropertyToID("_EmissionColor");
                    _initialColor = TargetRenderer.material.GetColor(_propertyID);
                    _currentColor = _initialColor;
                    break;
                case Methods.ShaderFloatValue:
                    _propertyID = Shader.PropertyToID(ShaderPropertyName);
                    _initialShaderFloatValue = TargetRenderer.material.GetFloat(_propertyID);
                    break;
            }
        }

        /// <summary>
        /// Resets blinking properties to original values
        /// </summary>
        protected virtual void ResetBlinkProperties()
        {
            _currentPhaseStartedAt = Time.time;
            CurrentPhaseIndex = 0;
            _repeatCount = RepeatCount;

            switch (Method)
            {
                case Methods.SetGameObjectActive:
                    TargetGameObject.SetActive(false);
                    break;
                case Methods.MaterialAlpha:
                    _currentColor = _initialColor;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
                case Methods.MaterialEmissionIntensity:
                    _currentColor = _initialColor;
                    TargetRenderer.material.SetColor(_propertyID, _currentColor);
                    break;
                case Methods.ShaderFloatValue:
                    TargetRenderer.material.SetFloat(_propertyID, _initialShaderFloatValue);
                    break;
            }
        }
    }
}
