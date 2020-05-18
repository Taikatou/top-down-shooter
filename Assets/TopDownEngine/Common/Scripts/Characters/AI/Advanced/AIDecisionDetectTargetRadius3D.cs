using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDetectTargetRadius3D")]
    public class AIDecisionDetectTargetRadius3D : AIDecision
    {
        /// the radius to search our target in
        public float Radius = 3f;
        /// the offset to apply (from the collider's center)
        public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
        /// the layer(s) to search our target on
        public LayerMask TargetLayerMask;
        /// whether or not we should look for obstacles
        public bool ObstacleDetection = true;
        /// the layer(s) to block the sight
        public LayerMask ObstacleMask;
        /// the frequency (in seconds) at which to check for obstacles
        public float TargetCheckFrequency = 1f;

        protected Collider _collider;
        protected Vector3 _raycastOrigin;
        protected Character _character;
        protected Color _gizmoColor = Color.yellow;
        protected bool _init = false;
        protected Vector3 _raycastDirection;
        protected float _lastTargetCheckTimestamp = 0f;
        protected Collider[] _hit;

        /// <summary>
        /// On init we grab our Character component
        /// </summary>
        public override void Initialization()
        {
            _character = this.gameObject.GetComponent<Character>();
            _collider = this.gameObject.GetComponent<Collider>();
            _gizmoColor.a = 0.25f;
            _init = true;
        }

        /// <summary>
        /// On Decide we check for our target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return DetectTarget();
        }

        /// <summary>
        /// Returns true if a target is found within the circle
        /// </summary>
        /// <returns></returns>
        protected virtual bool DetectTarget()
        {
            if (Time.time - _lastTargetCheckTimestamp < TargetCheckFrequency)
            {
                return false;
            }

            _lastTargetCheckTimestamp = Time.time;

            _raycastOrigin = _collider.bounds.center + DetectionOriginOffset / 2;
            _hit = Physics.OverlapSphere(_raycastOrigin, Radius, TargetLayerMask);

            if (_hit.Length > 0)
            {
                // we cast a ray to make sure there's no obstacle
                _raycastDirection = _hit[0].transform.position - _raycastOrigin;
                RaycastHit hit = MMDebug.Raycast3D(_raycastOrigin, _raycastDirection, Vector3.Distance(_hit[0].transform.position, _raycastOrigin), ObstacleMask.value, Color.yellow, true);
                if (hit.collider == null)
                {
                    _brain.Target = _hit[0].transform;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Draws gizmos for the detection circle
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            _raycastOrigin = transform.position + DetectionOriginOffset / 2;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_raycastOrigin, Radius);
            if (_init)
            {
                Gizmos.color = _gizmoColor;
                Gizmos.DrawSphere(_raycastOrigin, Radius);
            }            
        }
    }
}
