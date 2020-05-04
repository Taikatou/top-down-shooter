using System.Collections.Generic;
using MLAgents.Sensors;
using UnityEngine;

namespace Research.Common
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
                DebugDisplayInfo.RayInfo debugRay;
                RayPerceptionSensor.PerceiveSingleRay(rayInput, rayIndex, out debugRay);
                DrawRaycastGizmos(debugRay);
            }
        }

        public float GetRay()
        {
            DebugDisplayInfo.RayInfo output;
            var rayInput = GetRayPerceptionInput();
            RayPerceptionSensor.PerceiveSingleRay(rayInput, 0, out output);
            var rayOutput = output.rayOutput;
            return rayOutput.HasHit? rayOutput.HitFraction: 0;
        }
    
        void DrawRaycastGizmos(DebugDisplayInfo.RayInfo rayInfo, float alpha = 1.0f)
        {
            var startPositionWorld = rayInfo.worldStart;
            var endPositionWorld = rayInfo.worldEnd;
            var rayDirection = endPositionWorld - startPositionWorld;
            rayDirection *= rayInfo.rayOutput.HitFraction;

            // hit fraction ^2 will shift "far" hits closer to the hit color
            var lerpT = rayInfo.rayOutput.HitFraction * rayInfo.rayOutput.HitFraction;
            var color = Color.Lerp(rayHitColor, rayMissColor, lerpT);
            color.a *= alpha;
            Gizmos.color = color;
            Gizmos.DrawRay(startPositionWorld, rayDirection);

            // Draw the hit point as a sphere. If using rays to cast (0 radius), use a small sphere.
            if (rayInfo.rayOutput.HasHit)
            {
                var hitRadius = Mathf.Max(rayInfo.castRadius, .05f);
                Gizmos.DrawWireSphere(startPositionWorld + rayDirection, hitRadius);
            }
        }

        private RayPerceptionInput GetRayPerceptionInput()
        {
            var rayAngles = new [] {0.0f};

            var rayPerceptionInput = new RayPerceptionInput();
            rayPerceptionInput.RayLength = m_RayLength;
            rayPerceptionInput.DetectableTags = m_DetectableTags;
            rayPerceptionInput.Angles = rayAngles;
            rayPerceptionInput.StartOffset = 0.0f;
            rayPerceptionInput.EndOffset = 0.0f;
            rayPerceptionInput.CastRadius = 0.0f;
            rayPerceptionInput.Transform = transform;
            rayPerceptionInput.CastType = RayPerceptionCastType.Cast2D;
            rayPerceptionInput.LayerMask = m_RayLayerMask;

            return rayPerceptionInput;
        }
    }
}
