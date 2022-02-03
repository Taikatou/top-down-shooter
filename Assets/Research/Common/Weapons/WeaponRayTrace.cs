using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Research.Common.Weapons
{
    public class WeaponRayTrace : MonoBehaviour
    {
        [Range(1, 1000)]
        [Tooltip("Length of the rays to cast.")]
        public float m_RayLength = 20f;
    
        [SerializeField]
        [Header("Debug Gizmos", order = 999)]
        public Color rayHitColor = Color.red;
    
        [Tooltip("List of tags in the scene to compare against.")]
        public List<string> m_DetectableTags;
    
        [SerializeField]
        public Color rayMissColor = Color.white;
    
        [Tooltip("Controls which layers the rays can hit.")]
        public LayerMask m_RayLayerMask = Physics.DefaultRaycastLayers;
    
        void OnDrawGizmosSelected()
        {
            var rayInput = GetRayPerceptionInput();
            for (var rayIndex = 0; rayIndex < rayInput.Angles.Count; rayIndex++)
            {
                var outputRay = RayPerceptionSensor.PerceiveSingleRay(rayInput, rayIndex);
                DrawRaycastGizmos(outputRay);
            }
        }

        public float GetRay()
        {
            var rayInput = GetRayPerceptionInput();
            var rayOutput = RayPerceptionSensor.PerceiveSingleRay(rayInput, 0);
            return rayOutput.HasHit? rayOutput.HitFraction: 0;
        }
    
        void DrawRaycastGizmos(RayPerceptionOutput.RayOutput rayInfo, float alpha = 1.0f)
        {
            var startPositionWorld = rayInfo.StartPositionWorld;
            var endPositionWorld = rayInfo.EndPositionWorld;
            var rayDirection = endPositionWorld - startPositionWorld;
            rayDirection *= rayInfo.HitFraction;

            // hit fraction ^2 will shift "far" hits closer to the hit color
            var lerpT = rayInfo.HitFraction * rayInfo.HitFraction;
            var color = Color.Lerp(rayHitColor, rayMissColor, lerpT);
            color.a *= alpha;
            Gizmos.color = color;
            Gizmos.DrawRay(startPositionWorld, rayDirection);

            // Draw the hit point as a sphere. If using rays to cast (0 radius), use a small sphere.
            if (rayInfo.HasHit)
            {
                var hitRadius = Mathf.Max(rayInfo.ScaledCastRadius, .05f);
                Gizmos.DrawWireSphere(startPositionWorld + rayDirection, hitRadius);
            }
        }

        private RayPerceptionInput GetRayPerceptionInput()
        {
            var rayAngles = new [] {0.0f};

            var rayPerceptionInput = new RayPerceptionInput
            {
                RayLength = m_RayLength,
                DetectableTags = m_DetectableTags,
                Angles = rayAngles,
                StartOffset = 0.0f,
                EndOffset = 0.0f,
                CastRadius = 0.0f,
                Transform = transform,
                CastType = RayPerceptionCastType.Cast2D,
                LayerMask = m_RayLayerMask
            };

            return rayPerceptionInput;
        }
    }
}
