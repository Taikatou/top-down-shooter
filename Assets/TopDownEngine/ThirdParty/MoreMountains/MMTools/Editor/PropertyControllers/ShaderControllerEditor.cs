using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Custom editor for the ShaderController, conditional hiding 
    /// </summary>
    [CustomEditor(typeof(ShaderController), true)]
    [CanEditMultipleObjects]
    public class ShaderControllerEditor : Editor
    {
        protected SerializedProperty _TargetRenderer;
        protected SerializedProperty _Curve;
        protected SerializedProperty _MinValue;
        protected SerializedProperty _MaxValue;
        protected SerializedProperty _Duration;
        protected SerializedProperty _PingPongPauseDuration;
        protected SerializedProperty _Amplitude;
        protected SerializedProperty _Frequency;
        protected SerializedProperty _Shift;
        protected SerializedProperty _OneTimeDuration;
        protected SerializedProperty _OneTimeAmplitude;
        protected SerializedProperty _OneTimeRemapMin;
        protected SerializedProperty _OneTimeRemapMax;
        protected SerializedProperty _OneTimeCurve;
        protected SerializedProperty _OneTimeButton;
        protected SerializedProperty _DisableAfterOneTime;
        protected SerializedProperty _AudioAnalyzer;
        protected SerializedProperty _BeatID;
        protected SerializedProperty _AudioAnalyzerMultiplier;
        protected SerializedProperty _AudioAnalyzerOffset;
        protected SerializedProperty _AudioAnalyzerLerp;
        protected SerializedProperty _ToDestinationValue;
        protected SerializedProperty _ToDestinationDuration;
        protected SerializedProperty _ToDestinationCurve;
        protected SerializedProperty _ToDestinationButton;
        protected SerializedProperty _DisableAfterToDestination;
        protected SerializedProperty _InitialValue;
        protected SerializedProperty _CurrentValue;
        protected SerializedProperty _InitialColor;
        protected SerializedProperty _PropertyID;
        protected SerializedProperty _PropertyFound;
        protected SerializedProperty _TargetMaterial;

        /// <summary>
        /// On enable we grab our properties
        /// </summary>
        protected virtual void OnEnable()
        {
            ShaderController myTarget = (ShaderController)target;
            _TargetRenderer = serializedObject.FindProperty("TargetRenderer");
            _Curve = serializedObject.FindProperty("Curve");
            _MinValue = serializedObject.FindProperty("MinValue");
            _MaxValue = serializedObject.FindProperty("MaxValue");
            _Duration = serializedObject.FindProperty("Duration");
            _PingPongPauseDuration = serializedObject.FindProperty("PingPongPauseDuration");
            _Amplitude = serializedObject.FindProperty("Amplitude");
            _Frequency = serializedObject.FindProperty("Frequency");
            _Shift = serializedObject.FindProperty("Shift");
            _OneTimeDuration = serializedObject.FindProperty("OneTimeDuration");
            _OneTimeAmplitude = serializedObject.FindProperty("OneTimeAmplitude");
            _OneTimeRemapMin = serializedObject.FindProperty("OneTimeRemapMin");
            _OneTimeRemapMax = serializedObject.FindProperty("OneTimeRemapMax");
            _OneTimeCurve = serializedObject.FindProperty("OneTimeCurve");
            _DisableAfterOneTime = serializedObject.FindProperty("DisableAfterOneTime");
            _OneTimeButton = serializedObject.FindProperty("OneTimeButton");
            _AudioAnalyzer = serializedObject.FindProperty("AudioAnalyzer");
            _BeatID = serializedObject.FindProperty("BeatID");
            _AudioAnalyzerMultiplier = serializedObject.FindProperty("AudioAnalyzerMultiplier");
            _AudioAnalyzerOffset = serializedObject.FindProperty("AudioAnalyzerOffset");
            _AudioAnalyzerLerp = serializedObject.FindProperty("AudioAnalyzerLerp");
            _ToDestinationValue = serializedObject.FindProperty("ToDestinationValue");
            _ToDestinationDuration = serializedObject.FindProperty("ToDestinationDuration");
            _ToDestinationCurve = serializedObject.FindProperty("ToDestinationCurve");
            _DisableAfterToDestination = serializedObject.FindProperty("DisableAfterToDestination");
            _ToDestinationButton = serializedObject.FindProperty("ToDestinationButton");
            _InitialValue = serializedObject.FindProperty("InitialValue");
            _CurrentValue = serializedObject.FindProperty("CurrentValue");
            _InitialColor = serializedObject.FindProperty("InitialColor");
            _PropertyID = serializedObject.FindProperty("PropertyID");
            _PropertyFound = serializedObject.FindProperty("PropertyFound");
            _TargetMaterial = serializedObject.FindProperty("TargetMaterial");
        }

        /// <summary>
        /// Draws a conditional inspector
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "Modified ShaderController");

            ShaderController myTarget = (ShaderController)target;
                        
            Editor.DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "Curve", "MinValue", "MaxValue", "Duration", "PingPongPauseDuration",
                "Amplitude", "Frequency", "Shift", "OneTimeDuration", "OneTimeAmplitude", "OneTimeRemapMin",
            "OneTimeRemapMax", "OneTimeCurve", "DisableAfterOneTime", "OneTimeButton", "AudioAnalyzer", "BeatID", "AudioAnalyzerMultiplier", "AudioAnalyzerOffset",
                "AudioAnalyzerLerp", "ToDestinationValue",
            "ToDestinationDuration", "ToDestinationCurve", "DisableAfterToDestination", "ToDestinationButton",
            "InitialValue","CurrentValue","InitialColor","PropertyID","PropertyFound","TargetMaterial"});

            if (myTarget.ControlMode == ShaderController.ControlModes.PingPong)
            {
                EditorGUILayout.PropertyField(_Curve);
                EditorGUILayout.PropertyField(_MinValue);
                EditorGUILayout.PropertyField(_MaxValue);
                EditorGUILayout.PropertyField(_Duration);
                EditorGUILayout.PropertyField(_PingPongPauseDuration);
    }
            else if (myTarget.ControlMode == ShaderController.ControlModes.Random)
            {
                EditorGUILayout.PropertyField(_Amplitude);
                EditorGUILayout.PropertyField(_Frequency);
                EditorGUILayout.PropertyField(_Shift);
            }
            else if (myTarget.ControlMode == ShaderController.ControlModes.OneTime)
            {
                EditorGUILayout.PropertyField(_OneTimeDuration);
                EditorGUILayout.PropertyField(_OneTimeAmplitude);
                EditorGUILayout.PropertyField(_OneTimeRemapMin);
                EditorGUILayout.PropertyField(_OneTimeRemapMax);
                EditorGUILayout.PropertyField(_OneTimeCurve);
                EditorGUILayout.PropertyField(_DisableAfterOneTime);
                EditorGUILayout.PropertyField(_OneTimeButton);
            }
            else if (myTarget.ControlMode == ShaderController.ControlModes.AudioAnalyzer)
            {
                EditorGUILayout.PropertyField(_AudioAnalyzer);
                EditorGUILayout.PropertyField(_BeatID);
                EditorGUILayout.PropertyField(_AudioAnalyzerMultiplier);
                EditorGUILayout.PropertyField(_AudioAnalyzerOffset);
                EditorGUILayout.PropertyField(_AudioAnalyzerLerp);
            }
            else if (myTarget.ControlMode == ShaderController.ControlModes.ToDestination)
            {
                EditorGUILayout.PropertyField(_ToDestinationValue);
                EditorGUILayout.PropertyField(_ToDestinationDuration);
                EditorGUILayout.PropertyField(_ToDestinationCurve);
                EditorGUILayout.PropertyField(_DisableAfterToDestination);
                EditorGUILayout.PropertyField(_ToDestinationButton);
            }

            EditorGUILayout.PropertyField(_InitialValue);
            EditorGUILayout.PropertyField(_CurrentValue);
            EditorGUILayout.PropertyField(_InitialColor);
            EditorGUILayout.PropertyField(_PropertyID);
            EditorGUILayout.PropertyField(_PropertyFound);
            EditorGUILayout.PropertyField(_TargetMaterial);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
