using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{	
	[RequireComponent(typeof(Weapon))]
	/// <summary>
	/// Add this component to a Weapon and you'll be able to aim it (meaning you'll rotate it)
	/// Supported control modes are mouse, primary movement (you aim wherever you direct your character) and secondary movement (using a secondary axis, separate from the movement).
	/// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Aim 3D")]
	public class WeaponAim3D : WeaponAim
    {
        protected Vector2 _lastNonNullMovement;
        protected Vector2 _inputMovement;
        protected Camera _mainCamera;

        protected override void Initialization()
        {
            base.Initialization();
            _mainCamera = Camera.main;
            _lastNonNullMovement = new Vector2(0f, 1f);
        }

        /// <summary>
		/// Computes the current aim direction
		/// </summary>
		protected override void GetCurrentAim()
		{
			if (_weapon.Owner == null)
			{
				return;
			}

			if ((_weapon.Owner.LinkedInputManager == null) && (_weapon.Owner.CharacterType == Character.CharacterTypes.Player))
			{
				return;
			}

            AutoDetectWeaponMode();             

			switch (AimControl)
			{
				case AimControls.Off:
					if (_weapon.Owner == null) { return; }
                    GetOffAim();
                    break;

				case AimControls.Script:
                    GetScriptAim();
					break;

				case AimControls.PrimaryMovement:
					if (_weapon.Owner == null)
					{
						return;
                    }
                    GetPrimaryMovementAim();
                    break;

                case AimControls.SecondaryMovement:
                    if (_weapon.Owner == null) { return; }
                    GetSecondaryMovementAim();
                    break;

                case AimControls.SecondaryThenPrimaryMovement:
                    if (_weapon.Owner == null) { return; }

                    if (_weapon.Owner.LinkedInputManager.SecondaryMovement.magnitude > MinimumMagnitude)
                    {
                        GetSecondaryMovementAim();
                    }
                    else
                    {
                        GetPrimaryMovementAim();
                    }
                    break;

                case AimControls.PrimaryThenSecondaryMovement:
                    if (_weapon.Owner == null) { return; }

                    if (_weapon.Owner.LinkedInputManager.PrimaryMovement.magnitude > MinimumMagnitude)
                    {
                        GetPrimaryMovementAim();
                    }
                    else
                    {
                        GetSecondaryMovementAim();
                    }
                    break;

                case AimControls.Mouse:
					if (_weapon.Owner == null)
					{
						return;
					}
                    GetMouseAim();					
					break;			
			}
		}

        public virtual void GetOffAim()
        {
            _currentAim = Vector3.right;
            _direction = Vector3.right;
        }

        public virtual void GetPrimaryMovementAim()
        {
            _inputMovement = _weapon.Owner.LinkedInputManager.PrimaryMovement;
            _inputMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;

            _currentAim.x = _inputMovement.x;
            _currentAim.y = 0f;
            _currentAim.z = _inputMovement.y;
            _direction = transform.position + _currentAim;

            _lastNonNullMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;
        }

        public virtual void GetSecondaryMovementAim()
        {
            _inputMovement = _weapon.Owner.LinkedInputManager.SecondaryMovement;
            _inputMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;

            _currentAim.x = _inputMovement.x;
            _currentAim.y = 0f;
            _currentAim.z = _inputMovement.y;
            _direction = transform.position + _currentAim;

            _lastNonNullMovement = _inputMovement.magnitude > MinimumMagnitude ? _inputMovement : _lastNonNullMovement;
        }

        public virtual void GetScriptAim()
        {
            _direction = -(transform.position - _currentAim);
        }

        public virtual void GetMouseAim()
        {
            _mousePosition = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            float distance;
            if (_playerPlane.Raycast(ray, out distance))
            {
                Vector3 target = ray.GetPoint(distance);
                _direction = target;
            }

            _reticlePosition = _direction;

            if (Vector3.Distance(_direction, transform.position) < MouseDeadZoneRadius)
            {
                _direction = _lastMousePosition;
            }
            else
            {
                _lastMousePosition = _direction;
            }

            _direction.y = transform.position.y;
            _currentAim = _direction - transform.position;
        }

		/// <summary>
		/// Every frame, we compute the aim direction and rotate the weapon accordingly
		/// </summary>
		protected override void Update()
		{
			GetCurrentAim ();
			DetermineWeaponRotation ();
			MoveReticle();
			HideMousePointer ();
			UpdatePlane ();
		}

		protected virtual void UpdatePlane()
		{
			_playerPlane.SetNormalAndPosition (Vector3.up, this.transform.position);
		}

		/// <summary>
		/// Determines the weapon rotation based on the current aim direction
		/// </summary>
		protected override void DetermineWeaponRotation()
		{
			if (_currentAim != Vector3.zero)
			{
				if (_direction != Vector3.zero)
				{
					CurrentAngle = Mathf.Atan2 (_currentAim.z, _currentAim.x) * Mathf.Rad2Deg;
					if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
                    {
                        CurrentAngle = MMMaths.RoundToClosest (CurrentAngle, _possibleAngleValues);
                    }

                    // we add our additional angle
                    CurrentAngle += _additionalAngle;

					// we clamp the angle to the min/max values set in the inspector
					CurrentAngle = Mathf.Clamp (CurrentAngle, MinimumAngle, MaximumAngle);	
					CurrentAngle = -CurrentAngle + 90f;

					_lookRotation = Quaternion.Euler (CurrentAngle * Vector3.up);
                    RotateWeapon(_lookRotation);
				}
			}
			else
			{
				CurrentAngle = 0f;
				RotateWeapon(_initialRotation);	
			}
		}

        /// <summary>
        /// Initializes the reticle based on the settings defined in the inspector
        /// </summary>
        protected override void InitializeReticle()
        {
            if (_weapon.Owner == null) { return; }
            if (Reticle == null) { return; }
            if (ReticleType == ReticleTypes.None) { return; }

            if (ReticleType == ReticleTypes.Scene)
            {
                _reticle = (GameObject)Instantiate(Reticle);

                if (!ReticleAtMousePosition)
                {
                    if (_weapon.Owner != null)
                    {
                        _reticle.transform.SetParent(_weapon.transform);
                        _reticle.transform.localPosition = ReticleDistance * Vector3.forward;
                    }
                }
            }

            if (ReticleType == ReticleTypes.UI)
            {
                _reticle = (GameObject)Instantiate(Reticle);
                _reticle.transform.SetParent(GUIManager.Instance.MainCanvas.transform);
                _reticle.transform.localScale = Vector3.one;
                if (_reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>() != null)
                {
                    _reticle.gameObject.MMGetComponentNoAlloc<MMUIFollowMouse>().TargetCanvas = GUIManager.Instance.MainCanvas;
                }
            }
        }
        
        /// <summary>
        /// Every frame, moves the reticle if it's been told to follow the pointer
        /// </summary>
        protected override void MoveReticle()
		{		
			if (ReticleType == ReticleTypes.None) { return; }
			if (_reticle == null) { return; }
            if (_weapon.Owner.ConditionState.CurrentState == CharacterStates.CharacterConditions.Paused) { return; }

			if (ReticleType == ReticleTypes.Scene)
			{
				// if we're not supposed to rotate the reticle, we force its rotation, otherwise we apply the current look rotation
				if (!RotateReticle)
				{
					_reticle.transform.rotation = Quaternion.identity;
				}
				else
				{
					if (ReticleAtMousePosition)
					{
						_reticle.transform.rotation = _lookRotation;
                    }
                }

				// if we're in follow mouse mode and the current control scheme is mouse, we move the reticle to the mouse's position
				if (ReticleAtMousePosition && AimControl == AimControls.Mouse)
				{
					_reticle.transform.position = _reticlePosition;
                }

                if (MoveCameraTargetTowardsReticle && (_weapon.Owner != null))
                {
                    _newCamTargetPosition = Vector3.Lerp(_weapon.Owner.CameraTarget.transform.position, Vector3.Lerp(this.transform.position, _reticlePosition, CameraTargetOffset), Time.deltaTime * CameraTargetSpeed);
                    _newCamTargetDirection = _newCamTargetPosition - this.transform.position;
                    if (_newCamTargetDirection.magnitude > CameraTargetMaxDistance)
                    {
                        _newCamTargetDirection = _newCamTargetDirection.normalized * CameraTargetMaxDistance;
                    }
                    _newCamTargetPosition = this.transform.position + _newCamTargetDirection;
                    _weapon.Owner.CameraTarget.transform.position = _newCamTargetPosition;
                }
            }
		}

        /// <summary>
        /// Hides or show the mouse pointer based on the settings
        /// </summary>
		protected virtual void HideMousePointer()
		{
			if (GameManager.Instance.Paused)
			{
				Cursor.visible = true;
				return;
			}
			if (ReplaceMousePointer)
			{
				Cursor.visible = false;
			}
			else
			{
				Cursor.visible = true;
			}
		}
	}
}
