using MLAgents;
using MLAgents.Editor;
using MLAgents.Sensors;
using UnityEditor;
using UnityEngine;

namespace Research.Scripts
{
    /// <summary>
    /// A component for 2D Ray Perception.
    /// </summary>
    [AddComponentMenu("ML Agents/Top Down Ray Perception Sensor 2D", (int) MenuGroup.Sensors)]
    public class TopDownRaySensorComponent : RayPerceptionSensorComponentBase
    {
        public float verticalOffSet = 100.0f;
        /// <summary>
        /// Initializes the raycast sensor component.
        /// </summary>
        public TopDownRaySensorComponent()
        {
            // Set to the 2D defaults (just in case they ever diverge).
            rayLayerMask = Physics2D.DefaultRaycastLayers;
        }

        /// <inheritdoc/>
        public override RayPerceptionCastType GetCastType()
        {
            return RayPerceptionCastType.Cast2D;
        }

        public override float GetStartVerticalOffset()
        {
            return verticalOffSet;
        }
    }

    internal class TopDownRayPerceptionSensorComponentBaseEditor : RayPerceptionSensorComponentBaseEditor
    {
        [CustomEditor(typeof(TopDownRaySensorComponent))]
        [CanEditMultipleObjects]
        internal class TopDownRaySensorComponent2DEditor : RayPerceptionSensorComponentBaseEditor
        {
            public override void OnInspectorGUI()
            {
                OnRayPerceptionInspectorGUI(false);
            }

            protected override void AddProperties(SerializedObject so, bool is3d)
            {
                base.AddProperties(so, is3d);

                EditorGUILayout.PropertyField(so.FindProperty("verticalOffSet"), true);
            }
        }
    }
}