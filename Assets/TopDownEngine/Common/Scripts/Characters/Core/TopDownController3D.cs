using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace MoreMountains.TopDownEngine
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(CharacterController))]

    /// <summary>
    /// A controller, initially adapted from Unity's CharacterMotor.js, to add on top of Unity's native CharacterController, that will handle 
    /// </summary>
    public class TopDownController3D : TopDownController
    {
        [MMReadOnly]
        /// the current input sent to this character
        public Vector3 InputMoveDirection = Vector3.zero;

        /// the different possible update modes
        public enum UpdateModes { Update, FixedUpdate }
        /// the possible ways to transfer velocity on jump
        public enum VelocityTransferOnJump { NoTransfer, InitialVelocity, FloorVelocity, Relative }

        [Header("Settings")]
        /// whether the movement computation should occur at Update or FixedUpdate. FixedUpdate is the recommended choice.
        public UpdateModes UpdateMode = UpdateModes.FixedUpdate;
        /// how the velocity should be affected when jumping from a moving ground
        public VelocityTransferOnJump VelocityTransferMethod = VelocityTransferOnJump.FloorVelocity;

        [Header("Layer masks")]
        /// the layer to consider as ground
        public LayerMask GroundLayerMask;
        /// the layer to consider as obstacles (will prevent movement)
        public LayerMask ObstaclesLayerMask;

        [Header("Physics interaction")]
        /// the speed at which external forces get lerped to zero
        public float ImpactFalloff = 5f;
        /// the force to apply when colliding with rigidbodies
        public float PushPower = 2f;
        /// returns the center coordinate of the collider
        public override Vector3 ColliderCenter { get { return this.transform.position + _characterController.center; } }
        /// returns the bottom coordinate of the collider
        public override Vector3 ColliderBottom { get { return this.transform.position + _characterController.center + Vector3.down * _characterController.bounds.extents.y; } }
        /// returns the top coordinate of the collider
        public override Vector3 ColliderTop { get { return this.transform.position + _characterController.center + Vector3.up * _characterController.bounds.extents.y; } }
        /// whether or not the character is sliding down a steep slope
        public bool IsSliding() { return (Grounded && SlideOnSteepSurfaces && TooSteep()); }
        /// whether or not the character is colliding above
        public bool CollidingAbove() { return (_collisionFlags & CollisionFlags.CollidedAbove) != 0; }
        /// whether or not the current surface is too steep or not
        public bool TooSteep() { return (_groundNormal.y <= Mathf.Cos(_characterController.slopeLimit * Mathf.Deg2Rad)); }
        ///  whether or not the character is on a moving platform
        public override bool OnAMovingPlatform { get { return ShouldMoveWithPlatformThisFrame(); } }
        /// the speed of the moving platform
        public override Vector3 MovingPlatformSpeed
        {
            get { return _movingPlatformVelocity;  }
        }

            [Header("Movement")]
        /// the maximum vertical velocity the character can have while falling
        public float MaximumFallSpeed = 20.0f;
        /// the factor by which to multiply the speed while walking on a slope. x is the angle, y is the factor
        public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));

        [Header("Steep Surfaces")]
        /// whether or not the character should slide while standing on steep surfaces
        public bool SlideOnSteepSurfaces = true;
        /// the speed at which the character should slide
        public float SlidingSpeed = 15f;
        /// the control the player has on the speed while sliding down
        public float SlidingSpeedControl = 0.4f;
        /// the control the player has on the direction while sliding down
        public float SlidingDirectionControl = 1f;

        protected Transform _transform;
        protected Rigidbody _rigidBody;
        protected Collider _collider;
        protected CharacterController _characterController;
        protected RaycastHit _groundedRaycast;        
        protected float _originalColliderHeight;
        protected Vector3 _originalColliderCenter;
        protected Vector3 _originalSizeRaycastOrigin;
        protected Rigidbody _pushedRigidbody;
        protected Vector3 _pushDirection;
        protected Vector3 _groundNormal = Vector3.zero;
        protected Vector3 _lastGroundNormal = Vector3.zero;
        protected WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        // moving platforms
        protected Transform _movingPlatformHitCollider;
        protected Transform _movingPlatformCurrentHitCollider;
        protected Vector3 _movingPlatformCurrentHitColliderLocal;
        protected Vector3 _movingPlatformCurrentGlobalPoint;
        protected Quaternion _movingPlatformLocalRotation;
        protected Quaternion _movingPlatformGlobalRotation;
        protected Matrix4x4 _lastMovingPlatformMatrix;
        protected Vector3 _movingPlatformVelocity;
        protected bool _newMovingPlatform;

        // char movement
        protected CollisionFlags _collisionFlags;
        protected Vector3 _frameVelocity = Vector3.zero;
        protected Vector3 _hitPoint = Vector3.zero;
        protected Vector3 _lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

        // velocity
        protected Vector3 _newVelocity;
        protected Vector3 _lastHorizontalVelocity;
        protected Vector3 _newHorizontalVelocity;
        protected Vector3 _motion;
        protected Vector3 _idealVelocity;
        protected Vector3 _idealDirection;
        protected Vector3 _idealLocalDirection;
        protected Vector3 _horizontalVelocityDelta;
        protected float _stickyOffset;

        // move position
        protected RaycastHit _movePositionHit;
        protected Vector3 _capsulePoint1;
        protected Vector3 _capsulePoint2;
        protected Vector3 _movePositionDirection;
        protected float _movePositionDistance;

        // collision detection
        protected RaycastHit _raycastUp;
        protected RaycastHit _raycastDown;
        protected RaycastHit _raycastLeft;
        protected RaycastHit _raycastRight;

        /// <summary>
        /// On awake we store our various components for future use
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _characterController = GetComponent<CharacterController>();
            _transform = transform;
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _originalColliderHeight = _characterController.height;
            _originalColliderCenter = _characterController.center;
        }

        #region Update

        /// <summary>
        /// On late update we apply any impact we have in store, and store our velocity for use next frame
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();
            VelocityLastFrame = _rigidBody.velocity;
        }

        /// <summary>
        /// On Update we process our Update computations if UpdateMode is set to Update
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (UpdateMode == UpdateModes.Update)
            {
                ProcessUpdate();
            }
        }

        /// <summary>
        /// On FixedUpdate we process our Update computations if UpdateMode is set to FixedUpdate
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            ApplyImpact();
            GetMovingPlatformVelocity();
            if (UpdateMode == UpdateModes.FixedUpdate)
            {
                ProcessUpdate();
            }
        }
                
        /// <summary>
        /// Computes the new velocity and moves the character
        /// </summary>
        protected virtual void ProcessUpdate()
        {
            if (_transform == null)
            {
                return;
            }

            if (!FreeMovement)
            {
                return;
            }

            _newVelocity = Velocity;
            _positionLastFrame = _transform.position;

            AddInput();
            AddGravity();
            MoveWithPlatform();
            ComputeVelocityDelta();
            MoveCharacterController();
            DetectNewMovingPlatform();
            ComputeNewVelocity();
            StickToTheGround();
        }

        /// <summary>
        /// Determines the new velocity based on the slope we're on and the input 
        /// </summary>
        protected virtual void AddInput()
        {
            if (Grounded && TooSteep())
            {
                _idealVelocity.x = _groundNormal.x;
                _idealVelocity.y = 0;
                _idealVelocity.z = _groundNormal.z;
                _idealVelocity = _idealVelocity.normalized;
                _idealDirection = Vector3.Project(InputMoveDirection, _idealVelocity);
                _idealVelocity = _idealVelocity + (_idealDirection * SlidingSpeedControl) + (InputMoveDirection - _idealDirection) * SlidingDirectionControl;
                _idealVelocity = _idealVelocity * SlidingSpeed;
            }
            else
            {
                _idealVelocity = CurrentMovement;
            }

            if (VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity)
            {
                _idealVelocity += _frameVelocity;
                _idealVelocity.y = 0;
            }

            if (Grounded)
            {
                Vector3 sideways = Vector3.Cross(Vector3.up, _idealVelocity);
                _idealVelocity = Vector3.Cross(sideways, _groundNormal).normalized * _idealVelocity.magnitude;
            }

            _newVelocity = _idealVelocity;
            _newVelocity.y = Grounded ? Mathf.Min(_newVelocity.y, 0) : _newVelocity.y;
        }

        /// <summary>
        /// Adds the gravity to the new velocity and any AddedForce we may have
        /// </summary>
        protected virtual void AddGravity()
        {
            if (GravityActive)
            {
                if (Grounded)
                {
                    _newVelocity.y = Mathf.Min(0, _newVelocity.y) - Gravity * Time.deltaTime;
                }
                else
                {
                    _newVelocity.y = Velocity.y - Gravity * Time.deltaTime;
                    _newVelocity.y = Mathf.Max(_newVelocity.y, -MaximumFallSpeed);
                }
            }
            _newVelocity += AddedForce;
            AddedForce = Vector3.zero;
        }

        /// <summary>
        /// Moves and rotates the character controller to follow any moving platform we may be standing on
        /// </summary>
        protected virtual void MoveWithPlatform()
        {
            if (ShouldMoveWithPlatformThisFrame())
            {
                Vector3 moveDistance = Vector3.zero;
                Vector3 newGlobalPoint = _movingPlatformCurrentHitCollider.TransformPoint(_movingPlatformCurrentHitColliderLocal);
                moveDistance = (newGlobalPoint - _movingPlatformCurrentGlobalPoint);
                if (moveDistance != Vector3.zero)
                {
                    _characterController.Move(moveDistance);
                }
                Quaternion newGlobalRotation = _movingPlatformCurrentHitCollider.rotation * _movingPlatformLocalRotation;
                Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(_movingPlatformGlobalRotation);
                float yRotation = rotationDiff.eulerAngles.y;
                if (yRotation != 0)
                {
                    _transform.Rotate(0, yRotation, 0);
                }
            }
        }

        /// <summary>
        /// Computes the motion vector to apply to the character controller 
        /// </summary>
        protected virtual void ComputeVelocityDelta()
        {
            _motion = _newVelocity * Time.deltaTime;
            _horizontalVelocityDelta.x = _motion.x;
            _horizontalVelocityDelta.y = 0f;
            _horizontalVelocityDelta.z = _motion.z;
            _stickyOffset = Mathf.Max(_characterController.stepOffset, _horizontalVelocityDelta.magnitude);
            if (Grounded)
            {
                _motion -= _stickyOffset * Vector3.up;
            }
        }

        /// <summary>
        /// Moves the character controller by the computed _motion 
        /// </summary>
        protected virtual void MoveCharacterController()
        {
            _movingPlatformHitCollider = null;
            _groundNormal = Vector3.zero;

            _collisionFlags = _characterController.Move(_motion); // controller move

            _lastHitPoint = _hitPoint;
            _lastGroundNormal = _groundNormal;
        }

        /// <summary>
        /// Detects any moving platform we may be standing on
        /// </summary>
        protected virtual void DetectNewMovingPlatform()
        {
            if (_movingPlatformCurrentHitCollider != _movingPlatformHitCollider)
            {
                if (_movingPlatformHitCollider != null)
                {
                    _movingPlatformCurrentHitCollider = _movingPlatformHitCollider;
                    _lastMovingPlatformMatrix = _movingPlatformHitCollider.localToWorldMatrix;
                    _newMovingPlatform = true;
                }
            }
        }

        /// <summary>
        /// Determines the new Velocity value based on our position and our position last frame
        /// </summary>
        protected virtual void ComputeNewVelocity()
        {
            _lastHorizontalVelocity.x = _newVelocity.x;
            _lastHorizontalVelocity.y = 0;
            _lastHorizontalVelocity.z = _newVelocity.z;

            Velocity = (_transform.position - _positionLastFrame) / Time.deltaTime;

            _newHorizontalVelocity.x = Velocity.x;
            _newHorizontalVelocity.y = 0;
            _newHorizontalVelocity.z = Velocity.z;

            if (_lastHorizontalVelocity == Vector3.zero)
            {
                Velocity.x = 0f;
                Velocity.z = 0f;
            }
            else
            {
                float newVelocity = Vector3.Dot(_newHorizontalVelocity, _lastHorizontalVelocity) / _lastHorizontalVelocity.sqrMagnitude;
                Velocity = _lastHorizontalVelocity * Mathf.Clamp01(newVelocity) + Velocity.y * Vector3.up;
            }
            if (Velocity.y < _newVelocity.y - 0.001)
            {
                if (Velocity.y < 0)
                {
                    Velocity.y = _newVelocity.y;
                }
            }            
        }

        /// <summary>
        /// We make sure our character sticks to the ground
        /// </summary>
        protected virtual void StickToTheGround()
        {
            if (Grounded && !IsGroundedTest())
            {
                Grounded = false;
                if ((VelocityTransferMethod == VelocityTransferOnJump.InitialVelocity ||
                     VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity))
                {
                    _frameVelocity = _movingPlatformVelocity;
                    Velocity += _movingPlatformVelocity;
                }
                _transform.position += _stickyOffset * Vector3.up;
            }
            else if (!Grounded && IsGroundedTest())
            {
                Grounded = true;
                SubstractNewPlatformVelocity();
            }

            if (ShouldMoveWithPlatformThisFrame())
            {
                _movingPlatformCurrentHitColliderLocal = _movingPlatformCurrentHitCollider.InverseTransformPoint(_movingPlatformCurrentGlobalPoint);
                _movingPlatformGlobalRotation = _transform.rotation;
                _movingPlatformLocalRotation = Quaternion.Inverse(_movingPlatformCurrentHitCollider.rotation) * _movingPlatformGlobalRotation;
            }
        }

        /// <summary>
        /// Determines the direction based on the current movement
        /// </summary>
        protected override void DetermineDirection()
        {
            if (CurrentMovement.magnitude > 0f)
            {
                CurrentDirection = CurrentMovement.normalized;
            }
        }
        
        /// <summary>
        /// Handles friction with ground surfaces, coming soon
        /// </summary>
        protected override void HandleFriction()
        {
            //TODO - coming soon
        }

        #endregion

        #region Rigidbody push mechanics

        /// <summary>
        /// When collider with another controller, we store its info to handle moving platforms and push interactions
        /// </summary>
        /// <param name="hit"></param>
        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > 0 && hit.normal.y > _groundNormal.y && hit.moveDirection.y < 0)
            {
                if ((hit.point - _lastHitPoint).sqrMagnitude > 0.001 || _lastGroundNormal == Vector3.zero)
                {
                    _groundNormal = hit.normal;
                }
                else
                {
                    _groundNormal = _lastGroundNormal;
                }
                _movingPlatformHitCollider = hit.collider.transform;
                _hitPoint = hit.point;
                _frameVelocity = Vector3.zero;
            }

            HandlePush(hit, hit.point);
        }

        /// <summary>
        /// When triggering with something else, we check if it's a moving platform and we push ourselves if needed
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            // on trigger enter, if we're colliding with a moving platform, we push ourselves in the opposite direction
            if (other.gameObject.MMGetComponentNoAlloc<MovingPlatform3D>() != null)
            {
                if (this.transform.position.y < other.transform.position.y)
                {
                    Vector3 _pushbackDirection = (this.transform.position - other.transform.position).normalized;
                    this.Impact(_pushbackDirection.normalized, other.gameObject.MMGetComponentNoAlloc<MovingPlatform3D>().PushForce);
                }
            }
        }

        /// <summary>
        /// Adds a force to the colliding object at the hit position, to interact with the physics world
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="hitPosition"></param>
        protected virtual void HandlePush(ControllerColliderHit hit, Vector3 hitPosition)
        {

            _pushedRigidbody = hit.collider.attachedRigidbody;

            if ((_pushedRigidbody == null) || (_pushedRigidbody.isKinematic))
            {
                return;
            }
            
            _pushDirection.x = hit.moveDirection.x;
            _pushDirection.y = 0;
            _pushDirection.z = hit.moveDirection.z;

            _pushedRigidbody.AddForceAtPosition(_pushDirection * PushPower, hitPosition);
        }

        #endregion

        #region Moving Platforms
        
        /// <summary>
        /// Gets the current moving platform's velocity
        /// </summary>
        protected virtual void GetMovingPlatformVelocity()
        {
            if (_movingPlatformCurrentHitCollider != null)
            {
                if (!_newMovingPlatform)
                {
                    _movingPlatformVelocity = (
                        _movingPlatformCurrentHitCollider.localToWorldMatrix.MultiplyPoint3x4(_movingPlatformCurrentHitColliderLocal)
                        - _lastMovingPlatformMatrix.MultiplyPoint3x4(_movingPlatformCurrentHitColliderLocal)
                    ) / Time.deltaTime;
                }
                _lastMovingPlatformMatrix = _movingPlatformCurrentHitCollider.localToWorldMatrix;
                _newMovingPlatform = false;
            }
            else
            {
                _movingPlatformVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Computes the relative velocity
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator SubstractNewPlatformVelocity()
        {
            if ((VelocityTransferMethod == VelocityTransferOnJump.InitialVelocity ||
                 VelocityTransferMethod == VelocityTransferOnJump.FloorVelocity))
            {
                if (_newMovingPlatform)
                {
                    Transform platform = _movingPlatformCurrentHitCollider;
                    yield return _waitForFixedUpdate;
                    if (Grounded && platform == _movingPlatformCurrentHitCollider)
                    {
                        yield return 1;
                    }
                }
                Velocity -= _movingPlatformVelocity;
            }
        }

        /// <summary>
        /// Whether or not our character should move with the moving platform this frame
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldMoveWithPlatformThisFrame()
        {
            return (
                (Grounded || VelocityTransferMethod == VelocityTransferOnJump.Relative)
                && _movingPlatformCurrentHitCollider != null
            );
        }

        #endregion

        #region Collider Resizing

        /// <summary>
        /// Determines whether this instance can go back to original size.
        /// </summary>
        /// <returns><c>true</c> if this instance can go back to original size; otherwise, <c>false</c>.</returns>
        public override bool CanGoBackToOriginalSize()
        {
            // if we're already at original size, we return true
            if (_collider.bounds.size.y == _originalColliderHeight)
            {
                return true;
            }
            float headCheckDistance = _originalColliderHeight * transform.localScale.y * CrouchedRaycastLengthMultiplier;

            // we cast two rays above our character to check for obstacles. If we didn't hit anything, we can go back to original size, otherwise we can't
            _originalSizeRaycastOrigin = ColliderTop + transform.up * _smallValue;


            RaycastHit headCheck = MMDebug.Raycast3D(_originalSizeRaycastOrigin, transform.up, headCheckDistance, ObstaclesLayerMask, Color.cyan, true);
            if (headCheck.collider != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Resizes the collider to the new size set in parameters
        /// </summary>
        /// <param name="newSize">New size.</param>
        public override void ResizeCollider(float newHeight)
        {
            float newYOffset = _originalColliderCenter.y - (_originalColliderHeight - newHeight) / 2;
            _characterController.height = newHeight;
            _characterController.center = newYOffset * Vector3.up;
            this.transform.Translate((newYOffset / 2f) * Vector3.up);
        }

        /// <summary>
        /// Returns the collider to its initial size
        /// </summary>
        public override void ResetColliderSize()
        {
            _characterController.height = _originalColliderHeight;
            _characterController.center = _originalColliderCenter;
        }

        #endregion

        #region Grounded Tests

        /// <summary>
        /// Whether or not the character is grounded
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsGroundedTest()
        {
            return (_groundNormal.y > 0.01);
        }

        /// <summary>
        /// Grounded check
        /// </summary>
        protected override void CheckIfGrounded()
        {
            JustGotGrounded = (!_groundedLastFrame && Grounded);
            _groundedLastFrame = Grounded;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables the collider
        /// </summary>
        public override void CollisionsOn()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// Disables collider
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
            _raycastRight = MMDebug.Raycast3D(this.transform.position + offset, Vector3.right, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastRight.collider != null) { DetectedObstacleRight = _raycastRight.collider.gameObject; } else { DetectedObstacleRight = null; }
            _raycastLeft = MMDebug.Raycast3D(this.transform.position + offset, Vector3.left, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastLeft.collider != null) { DetectedObstacleLeft = _raycastLeft.collider.gameObject; } else { DetectedObstacleLeft = null; }
            _raycastUp = MMDebug.Raycast3D(this.transform.position + offset, Vector3.forward, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastUp.collider != null) { DetectedObstacleUp = _raycastUp.collider.gameObject; } else { DetectedObstacleUp = null; }
            _raycastDown = MMDebug.Raycast3D(this.transform.position + offset, Vector3.back, distance, ObstaclesLayerMask, Color.yellow, true);
            if (_raycastDown.collider != null) { DetectedObstacleDown = _raycastDown.collider.gameObject; } else { DetectedObstacleDown = null; }
        }

        /// <summary>
        /// Applies the stored impact to the character
        /// </summary>
        protected virtual void ApplyImpact()
        {
            if (_impact.magnitude > 0.2f)
            {
                _characterController.Move(_impact * Time.deltaTime);
            }
            _impact = Vector3.Lerp(_impact, Vector3.zero, ImpactFalloff * Time.deltaTime);
        }

        /// <summary>
        /// Adds a force of the specified direction and magnitude to the character
        /// </summary>
        /// <param name="movement"></param>
        public override void AddForce(Vector3 movement)
        {
            AddedForce += movement;
        }

        /// <summary>
        /// Applies an impact to the character of the specified direction and force
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="force"></param>
        public override void Impact(Vector3 direction, float force)
        {
            direction = direction.normalized;
            if (direction.y < 0) { direction.y = -direction.y; }
            _impact += direction.normalized * force;
        }

        /// <summary>
        /// Sets the character's current input direction and magnitude
        /// </summary>
        /// <param name="movement"></param>
        public override void SetMovement(Vector3 movement)
        {
            CurrentMovement = movement;

            Vector3 directionVector;
            directionVector = movement;
            if (directionVector != Vector3.zero)
            {
                float directionLength = directionVector.magnitude;
                directionVector = directionVector / directionLength;
                directionLength = Mathf.Min(1, directionLength);
                directionLength = directionLength * directionLength;
                directionVector = directionVector * directionLength;
            }
            InputMoveDirection = transform.rotation * directionVector;
        }

        /// <summary>
        /// Turns this character's rigidbody kinematic or not
        /// </summary>
        /// <param name="state"></param>
        public override void SetKinematic(bool state)
        {
            _rigidBody.isKinematic = state;
        }

        /// <summary>
        /// Moves this character to the specified position while trying to avoid obstacles
        /// </summary>
        /// <param name="newPosition"></param>
        public override void MovePosition(Vector3 newPosition)
        {
            
            _movePositionDirection = (newPosition - this.transform.position);
            _movePositionDistance = Vector3.Distance(this.transform.position, newPosition) ;

            _capsulePoint1 =    this.transform.position 
                                + _characterController.center 
                                - (Vector3.up * _characterController.height / 2f) 
                                + Vector3.up * _characterController.skinWidth 
                                + Vector3.up * _characterController.radius;
            _capsulePoint2 =    this.transform.position
                                + _characterController.center
                                + (Vector3.up * _characterController.height / 2f)
                                - Vector3.up * _characterController.skinWidth
                                - Vector3.up * _characterController.radius;

            if (!Physics.CapsuleCast(_capsulePoint1, _capsulePoint2, _characterController.radius, _movePositionDirection, out _movePositionHit, _movePositionDistance, ObstaclesLayerMask))
            {
                this.transform.position = newPosition;
            }
        }

        #endregion
        
    }
}