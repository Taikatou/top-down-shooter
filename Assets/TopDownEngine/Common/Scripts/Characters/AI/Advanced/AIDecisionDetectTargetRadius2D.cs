using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This decision will return true if an object on its TargetLayer layermask is within its specified radius, false otherwise. It will also set the Brain's Target to that object.
    /// </summary>
    public class AIDecisionDetectTargetRadius2D : AIDecision
    {
        /// the radius to search our target in
        public float Radius = 3f;
        /// the center of the search circle
        public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);
        /// the layer(s) to search our target on
        public LayerMask TargetLayer;
        /// whether or not to look for obstacles
        public bool ObstacleDetection = true;
        /// the layer(s) to look for obstacles on
        public LayerMask ObstacleMask;

        protected Collider2D _collider;
        protected Vector2 _facingDirection;
        protected Vector2 _raycastOrigin;
        protected Character _character;
        protected CharacterOrientation2D _orientation2D;
        protected Collider2D _detectionCollider = null;
        protected Color _gizmoColor = Color.yellow;
        protected bool _init = false;
        protected Vector2 _boxcastDirection;

        /// <summary>
        /// On init we grab our Character component
        /// </summary>
        public override void Initialization()
        {
            _character = this.gameObject.GetComponent<Character>();
            _orientation2D = this.gameObject.GetComponent<CharacterOrientation2D>();
            _collider = this.gameObject.GetComponent<Collider2D>();
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
            _detectionCollider = null;
            
            if (_orientation2D != null)
            {
                _facingDirection = _orientation2D.IsFacingRight ? Vector2.right : Vector2.left;
                _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
                _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;
            }
            else
            {
                _raycastOrigin = transform.position +  DetectionOriginOffset;
            }
            
            // we cast a ray to the left of the agent to check for a Player

            _detectionCollider = Physics2D.OverlapCircle(_raycastOrigin, Radius, TargetLayer);     
            if (_detectionCollider == null)
            {
                return false;
            }
            else
            {
                // we cast a ray to make sure there's no obstacle
                _boxcastDirection = (Vector2)(_detectionCollider.gameObject.MMGetComponentNoAlloc<Collider2D>().bounds.center - _collider.bounds.center);
                RaycastHit2D hit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, _boxcastDirection.normalized, _boxcastDirection.magnitude, ObstacleMask);
                if (!hit)
                {
                    _brain.Target = _detectionCollider.gameObject.transform;
                    return true;
                }
                else
                {
                    return false;
                }                
            }
        }

        /// <summary>
        /// Draws gizmos for the detection circle
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
            _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

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
