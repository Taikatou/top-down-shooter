﻿using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// This decision returns true if there's no obstacle in a straight line between the agent and the brain's target, in 3D
    /// </summary>
    public class AIDecisionLineOfSightToTarget3D : AIDecision
    {
        /// the layermask to consider as obstacles when trying to determine whether a line of sight is present
        public LayerMask ObstacleLayerMask;
        /// the offset to apply (from the collider's center) when casting a ray from the agent to its target
        public Vector3 LineOfSightOffset = new Vector3(0, 0, 0);

        protected Vector3 _directionToTarget;
        protected Collider _collider;
        protected Vector3 _raycastOrigin;

        /// <summary>
        /// On init we grab our collider
        /// </summary>
        public override void Initialization()
        {
            _collider = this.gameObject.GetComponent<Collider>();
        }

        /// <summary>
        /// On Decide we check whether we have a line of sight
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return CheckLineOfSight();
        }

        /// <summary>
        /// Checks whether there are obstacles between the agent and the target
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckLineOfSight()
        {
            if (_brain.Target == null)
            {
                return false;
            }

            _raycastOrigin = _collider.bounds.center + LineOfSightOffset / 2;
            _directionToTarget = _brain.Target.transform.position - _raycastOrigin;
            
            RaycastHit hit = MMDebug.Raycast3D(_raycastOrigin, _directionToTarget.normalized, _directionToTarget.magnitude, ObstacleLayerMask, Color.yellow, true);
            if (hit.collider == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
