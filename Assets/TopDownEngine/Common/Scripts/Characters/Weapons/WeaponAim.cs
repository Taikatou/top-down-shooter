using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    [RequireComponent(typeof(Weapon))]
    public class WeaponAim : MonoBehaviour
    {
        /// the list of possible control modes
        public enum AimControls { Off, PrimaryMovement, SecondaryMovement, Mouse, Script, SecondaryThenPrimaryMovement, PrimaryThenSecondaryMovement }
        /// the list of possible rotation modes
        public enum RotationModes { Free, Strict2Directions, Strict4Directions, Strict8Directions }
        /// the possible types of reticles
        public enum ReticleTypes { None, Scene, UI }

        [Header("Control Mode")]
        [Information("Add this component to a Weapon and you'll be able to aim (rotate) it. It supports three different control modes : mouse (the weapon aims towards the pointer), primary movement (you'll aim towards the current input direction), or secondary movement (aims towards a second input axis, think twin stick shooters).", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// the aim control mode
        public AimControls AimControl = AimControls.SecondaryMovement;

        [Header("Weapon Rotation")]
        [Information("Here you can define whether the rotation is free, strict in 4 directions (top, bottom, left, right), or 8 directions (same + diagonals). You can also define a rotation speed, and a min and max angle. For example, if you don't want your character to be able to aim in its back, set min angle to -90 and max angle to 90.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// the rotation mode
        public RotationModes RotationMode = RotationModes.Free;
        /// the the speed at which the weapon reaches its new position. Set it to zero if you want movement to directly follow input
        public float WeaponRotationSpeed = 1f;
        [Range(-180, 180)]
        /// the minimum angle at which the weapon's rotation will be clamped
        public float MinimumAngle = -180f;
        [Range(-180, 180)]
        /// the maximum angle at which the weapon's rotation will be clamped
        public float MaximumAngle = 180f;
        /// the minimum threshold at which the weapon's rotation magnitude will be considered 
        public float MinimumMagnitude = 0.2f;

        [Header("Reticle")]
        [Information("You can also display a reticle on screen to check where you're aiming at. Leave it blank if you don't want to use one. If you set the reticle distance to 0, it'll follow the cursor, otherwise it'll be on a circle centered on the weapon. You can also ask it to follow the mouse, even replace the mouse pointer. You can also decide if the pointer should rotate to reflect aim angle or remain stable.", MoreMountains.Tools.InformationAttribute.InformationType.Info, false)]
        /// Defines whether the reticle is placed in the scene or in the UI
        public ReticleTypes ReticleType = ReticleTypes.None;
        /// the gameobject to display as the aim's reticle/crosshair. Leave it blank if you don't want a reticle
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public GameObject Reticle;
        /// the distance at which the reticle will be from the weapon
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public float ReticleDistance;
        /// if set to true, the reticle will be placed at the mouse's position (like a pointer)
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public bool ReticleAtMousePosition;
        /// if set to true, the reticle will rotate on itself to reflect the weapon's rotation. If not it'll remain stable.
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public bool RotateReticle = false;
        /// if set to true, the reticle will replace the mouse pointer
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public bool ReplaceMousePointer = true;
        /// the radius around the weapon rotation centre where the mouse will be ignored, to avoid glitches
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public float MouseDeadZoneRadius = 0.5f;
        /// if set to false, the reticle won't be added and displayed
        [EnumCondition("ReticleType", (int)ReticleTypes.Scene, (int)ReticleTypes.UI)]
        public bool DisplayReticle = true;

        [Header("CameraTarget")]
        public bool MoveCameraTargetTowardsReticle = false;
        [Range(0f, 1f)]
        public float CameraTargetOffset = 0.3f;
        [Condition("MoveCameraTargetTowardsReticle", true)]
        public float CameraTargetMaxDistance = 10f;
        [Condition("MoveCameraTargetTowardsReticle", true)]
        public float CameraTargetSpeed = 5f;

        public float CurrentAngleAbsolute { get; protected set; }
        /// the weapon's current rotation
		public Quaternion CurrentRotation { get { return transform.rotation; } }
        /// the weapon's current direction
        public Vector3 CurrentAim { get { return _currentAim; } }
        /// the weapon's current direction, absolute (flip independent)
        public Vector3 CurrentAimAbsolute { get { return _currentAimAbsolute; } }
        /// the current angle the weapon is aiming at
        public float CurrentAngle { get; protected set; }
        /// the current angle the weapon is aiming at, adjusted to compensate for the current orientation of the character
        public virtual float CurrentAngleRelative
        {
            get
            {
                if (_weapon != null)
                {
                    if (_weapon.Owner != null)
                    {
                        return CurrentAngle;
                    }
                }
                return 0;
            }
        }

        protected Weapon _weapon;
        protected Vector3 _currentAim = Vector3.zero;
        protected Vector3 _currentAimAbsolute = Vector3.zero;
        protected Quaternion _lookRotation;
        protected Vector3 _direction;
        protected float[] _possibleAngleValues;
        protected Vector3 _mousePosition;
        protected Vector3 _lastMousePosition;
        protected float _additionalAngle;
        protected Quaternion _initialRotation;
        protected Plane _playerPlane;
        protected GameObject _reticle;
        protected Vector3 _reticlePosition;
        protected Vector3 _newCamTargetPosition;
        protected Vector3 _newCamTargetDirection;

        /// <summary>
        /// On Start(), we trigger the initialization
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
		/// Grabs the weapon component, initializes the angle values
		/// </summary>
		protected virtual void Initialization()
        {
            _weapon = GetComponent<Weapon>();

            if (RotationMode == RotationModes.Strict4Directions)
            {
                _possibleAngleValues = new float[5];
                _possibleAngleValues[0] = -180f;
                _possibleAngleValues[1] = -90f;
                _possibleAngleValues[2] = 0f;
                _possibleAngleValues[3] = 90f;
                _possibleAngleValues[4] = 180f;
            }
            if (RotationMode == RotationModes.Strict8Directions)
            {
                _possibleAngleValues = new float[9];
                _possibleAngleValues[0] = -180f;
                _possibleAngleValues[1] = -135f;
                _possibleAngleValues[2] = -90f;
                _possibleAngleValues[3] = -45f;
                _possibleAngleValues[4] = 0f;
                _possibleAngleValues[5] = 45f;
                _possibleAngleValues[6] = 90f;
                _possibleAngleValues[7] = 135f;
                _possibleAngleValues[8] = 180f;
            }
            _initialRotation = transform.rotation;
            InitializeReticle();
            _playerPlane = new Plane(Vector3.up, Vector3.zero);
        }

        /// <summary>
		/// Aims the weapon towards a new point
		/// </summary>
		/// <param name="newAim">New aim.</param>
		public virtual void SetCurrentAim(Vector3 newAim)
        {
            _currentAim = newAim;
        }


        protected virtual void GetCurrentAim()
        {

        }

        /// <summary>
        /// Every frame, we compute the aim direction and rotate the weapon accordingly
        /// </summary>
        protected virtual void Update()
        {

        }

        /// <summary>
        /// On LateUpdate, resets any additional angle
        /// </summary>
        protected virtual void LateUpdate()
        {
            ResetAdditionalAngle();
        }
        
        /// <summary>
        /// Determines the weapon's rotation
        /// </summary>
        protected virtual void DetermineWeaponRotation()
        {

        }

        /// <summary>
        /// Moves the weapon's reticle
        /// </summary>
        protected virtual void MoveReticle()
        {

        }

        /// <summary>
        /// Rotates the weapon, optionnally applying a lerp to it.
        /// </summary>
        /// <param name="newRotation">New rotation.</param>
        protected virtual void RotateWeapon(Quaternion newRotation)
        {
            if (GameManager.Instance.Paused)
            {
                return;
            }
            // if the rotation speed is == 0, we have instant rotation
            if (WeaponRotationSpeed == 0)
            {
                transform.rotation = newRotation;
            }
            // otherwise we lerp the rotation
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, WeaponRotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
		/// If a reticle has been set, instantiates the reticle and positions it
		/// </summary>
		protected virtual void InitializeReticle()
        {
           
        }

        /// <summary>
        /// Removes any remaining reticle
        /// </summary>
        public virtual void RemoveReticle()
        {
            if (_reticle != null)
            {
                Destroy(_reticle.gameObject);
            }
        }

        /// <summary>
        /// Adds additional angle to the weapon's rotation
        /// </summary>
        /// <param name="addedAngle"></param>
        public virtual void AddAdditionalAngle(float addedAngle)
        {
            _additionalAngle += addedAngle;
        }

        /// <summary>
        /// Resets the additional angle
        /// </summary>
        protected virtual void ResetAdditionalAngle()
        {
            _additionalAngle = 0;
        }

        protected virtual void AutoDetectWeaponMode()
        {
            if (_weapon.Owner.LinkedInputManager != null)
            {
                if ((_weapon.Owner.LinkedInputManager.ForceWeaponMode) && (AimControl != AimControls.Off))
                {
                    AimControl = _weapon.Owner.LinkedInputManager.WeaponForcedMode;
                }

                if ((!_weapon.Owner.LinkedInputManager.ForceWeaponMode) && (_weapon.Owner.LinkedInputManager.IsMobile) && (AimControl == AimControls.Mouse))
                {
                    AimControl = AimControls.PrimaryMovement;
                }
            }
        }
    }
}
