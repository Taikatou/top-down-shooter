using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// a controller to move a rigidbody2D and collider2D around in top down view
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Core/TopDown Controller 2D")]
    public class TopDownController2D : TopDownController 
	{
        [MMReadOnly]
        /// whether or not the character is above a hole right now
        public bool OverHole = false;
        /// the collider's center position   
        public override Vector3 ColliderCenter { get { return (Vector2)this.transform.position + _collider.offset; } }
        /// the collider's bottom position
        public override Vector3 ColliderBottom { get { return (Vector2)this.transform.position + _collider.offset + Vector2.down * _collider.bounds.extents.y; } }
        /// the collider's top position
        public override Vector3 ColliderTop { get { return (Vector2)this.transform.position + _collider.offset + Vector2.up * _collider.bounds.extents.y; } }
        ///  whether or not the character is on a moving platform
        public override bool OnAMovingPlatform { get { return _movingPlatform; } }
        /// the speed of the moving platform
        public override Vector3 MovingPlatformSpeed { get { if (_movingPlatform != null) { return _movingPlatform.CurrentSpeed; } else { return Vector3.zero; } } }

        /// the layer mask to consider as ground
        public LayerMask GroundLayerMask;
        /// the layer mask to consider as holes
        public LayerMask HoleLayerMask;
        /// the layer to consider as obstacles (will prevent movement)
        public LayerMask ObstaclesLayerMask;

        protected Rigidbody2D _rigidBody;
        protected BoxCollider2D _collider;
        protected Vector2 _originalColliderSize;
        protected Vector3 _originalColliderCenter;
        protected Vector3 _originalSizeRaycastOrigin;
        protected Vector3 _orientedMovement;
        protected Collider2D _groundedTest;
        protected Collider2D _holeTestMin;
        protected Collider2D _holeTestMax;
        protected MovingPlatform2D _movingPlatform;
        protected Vector3 _movingPlatformPositionLastFrame;

        // collision detection
        protected RaycastHit2D _raycastUp;
        protected RaycastHit2D _raycastDown;
        protected RaycastHit2D _raycastLeft;
        protected RaycastHit2D _raycastRight;

        /// <summary>
        /// On awake we grabd our components
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _originalColliderSize = _collider.size;
            _originalColliderCenter = _collider.offset;
        }

        /// <summary>
        /// Determines whether or not this character is grounded
        /// </summary>
        protected override void CheckIfGrounded()
        {
            _groundedTest = Physics2D.OverlapPoint((Vector2)this.transform.position, GroundLayerMask);
            _holeTestMin = Physics2D.OverlapPoint((Vector2)_collider.bounds.min, HoleLayerMask);
            _holeTestMax = Physics2D.OverlapPoint((Vector2)_collider.bounds.max, HoleLayerMask);
            Grounded = (_groundedTest != null);
            OverHole = ((_holeTestMin != null) && (_holeTestMax != null));                        
            JustGotGrounded = (!_groundedLastFrame && Grounded);
            _groundedLastFrame = Grounded;
        }

        /// <summary>
        /// On update we determine our acceleration
        /// </summary>
        protected override void Update()
        {
            base.Update();
            Velocity = _rigidBody.velocity;
            Acceleration = (_rigidBody.velocity - (Vector2)VelocityLastFrame) / Time.fixedDeltaTime;
        }

        /// <summary>
        /// On late update, we apply an impact
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();
            VelocityLastFrame = _rigidBody.velocity;
        }

        /// <summary>
        /// Handles the friction, still a work in progress (todo)
        /// </summary>
        protected override void HandleFriction()
        {
            if (SurfaceModifierBelow == null)
            {
                Friction = 0f;
                AddedForce = Vector3.zero;
                return;
            }
            else
            {
                Friction = SurfaceModifierBelow.Friction;

                if (AddedForce.y != 0f)
                {
                    AddForce(AddedForce);
                }

                AddedForce.y = 0f;
                AddedForce = SurfaceModifierBelow.AddedForce;
            }
        }

        /// <summary>
        /// On fixed update, we move our rigidbody 
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            ApplyImpact();

            if (!FreeMovement)
            {
                return;
            }

            if (Friction > 1)
            {
                CurrentMovement = CurrentMovement / Friction;
            }
            
            // if we have a low friction (ice, marbles...) we lerp the speed accordingly
            if (Friction > 0 && Friction < 1)
            {
                CurrentMovement = Vector3.Lerp(Speed, CurrentMovement, Time.deltaTime * Friction);
            }
            
            Vector2 newMovement = _rigidBody.position + (Vector2)(CurrentMovement + AddedForce) * Time.fixedDeltaTime;
            
            if (OnAMovingPlatform)
            {
                newMovement += (Vector2)_movingPlatform.CurrentSpeed * Time.fixedDeltaTime;
            }
            _rigidBody.MovePosition(newMovement);
        }

        /// <summary>
        /// Another way to add a force of the specified force and direction
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="force"></param>
        public override void Impact(Vector3 direction, float force)
        {
            direction = direction.normalized;
            _impact += direction.normalized * force;
        }

        /// <summary>
        /// Applies the current impact
        /// </summary>
        protected virtual void ApplyImpact()
        {
            if (_impact.magnitude > 0.2f)
            {
                _rigidBody.AddForce(_impact);
            }
            _impact = Vector3.Lerp(_impact, Vector3.zero, 5f * Time.deltaTime);
        }

        /// <summary>
        /// Adds a force of the specified vector
        /// </summary>
        /// <param name="movement"></param>
        public override void AddForce(Vector3 movement)
        {
            Impact(movement.normalized, movement.magnitude);
        }
        
        /// <summary>
        /// Sets the current movement
        /// </summary>
        /// <param name="movement"></param>
        public override void SetMovement(Vector3 movement)
        {
            _orientedMovement = movement;
            _orientedMovement.y = _orientedMovement.z;
            _orientedMovement.z = 0f;
            CurrentMovement = _orientedMovement;
        }

        /// <summary>
        /// Tries to move to the specified position
        /// </summary>
        /// <param name="newPosition"></param>
        public override void MovePosition(Vector3 newPosition)
        {
            _rigidBody.MovePosition(newPosition);
        }

        /// <summary>
        /// Resizes the collider to the new size set in parameters
        /// </summary>
        /// <param name="newSize">New size.</param>
        public override void ResizeCollider(float newHeight)
        {
            float newYOffset = _originalColliderCenter.y - (_originalColliderSize.y - newHeight) / 2;
            Vector2 newSize = _collider.size;
            newSize.y = newHeight;
            _collider.size = newSize;
            _collider.offset = newYOffset * Vector3.up;
        }

        /// <summary>
        /// Returns the collider to its initial size
        /// </summary>
        public override void ResetColliderSize()
        {
            _collider.size = _originalColliderSize;
            _collider.offset = _originalColliderCenter;
        }
        
        /// <summary>
        /// Determines the controller's current direction
        /// </summary>
        protected override void DetermineDirection()
        {
            if (CurrentMovement != Vector3.zero)
            {
                CurrentDirection = CurrentMovement.normalized;
            }
        }

        /// <summary>
        /// Sets a moving platform to this controller
        /// </summary>
        /// <param name="platform"></param>
        public virtual void SetMovingPlatform(MovingPlatform2D platform)
        {
            _movingPlatform = platform;
        }

        /// <summary>
        /// Sets this rigidbody as kinematic
        /// </summary>
        /// <param name="state"></param>
        public override void SetKinematic(bool state)
        {
            _rigidBody.isKinematic = state;
        }

        /// <summary>
        /// Enables the collider
        /// </summary>
        public override void CollisionsOn()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// Disables the collider
        /// </summary>
        public override void CollisionsOff()
        {
            _collider.enabled = false;
        }

        /// <summary>
        /// Performs a cardinal collision check and stores collision objects informations
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="offset"></param>
        public override void DetectObstacles(float distance, Vector3 offset)
        {
            _raycastRight = MMDebug.RayCast(this.transform.position + offset, Vector3.right, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastRight.collider != null) { DetectedObstacleRight = _raycastRight.collider.gameObject; } else { DetectedObstacleRight = null; }
            _raycastLeft = MMDebug.RayCast(this.transform.position + offset, Vector3.left, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastLeft.collider != null) { DetectedObstacleLeft = _raycastLeft.collider.gameObject; } else { DetectedObstacleLeft = null; }
            _raycastUp = MMDebug.RayCast(this.transform.position + offset, Vector3.up, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastUp.collider != null) { DetectedObstacleUp = _raycastUp.collider.gameObject; } else { DetectedObstacleUp = null; }
            _raycastDown = MMDebug.RayCast(this.transform.position + offset, Vector3.down, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastDown.collider != null) { DetectedObstacleDown = _raycastDown.collider.gameObject; } else { DetectedObstacleDown = null; }
        }
    }
}
