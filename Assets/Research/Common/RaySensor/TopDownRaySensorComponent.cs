using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.RaySensor
{
    /// <summary>
    /// A component for 2D Ray Perception.
    /// </summary>
    [AddComponentMenu("ML Agents/Top Down Ray Perception Sensor 2D", 50)]
    public class TopDownRaySensorComponent : RayPerceptionSensorComponentBase
    {
        public float verticalOffSet = 100.0f;
        /// <summary>
        /// Initializes the raycast sensor component.
        /// </summary>
        public TopDownRaySensorComponent()
        {
            // Set to the 2D defaults (just in case they ever diverge).
            RayLayerMask = Physics2D.DefaultRaycastLayers;
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
}
