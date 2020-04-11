using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Add this class to a weapon and it'll prevent shooting when close to an obstacle (as defined by the ObstacleLayerMask)
    /// </summary>
    [RequireComponent(typeof(Weapon))]
    [AddComponentMenu("TopDown Engine/Weapons/Weapon Prevent Shooting when Close to Walls 3D")]
    public class WeaponPreventShootingWhenCloseToWalls3D : MonoBehaviour
    {
        /// the angle to consider when deciding whether or not there's a wall in front of the weapon (usually 5 degrees is fine)
        public float Angle = 5f;
        /// the max distance to the wall we want to prevent shooting from
        public float Distance = 2f;
        /// the offset to apply to the detection (in addition and relative to the weapon's position)
        public Vector3 RaycastOriginOffset = Vector3.zero;
        /// the layers to consider as obstacles
        public LayerMask ObstacleLayerMask;

        protected RaycastHit _hitLeft;
        protected RaycastHit _hitMiddle;
        protected RaycastHit _hitRight;
        protected Weapon _weapon;
        protected CharacterHandleWeapon _characterHandleWeapon;
        protected bool _shootStopped = false;

        /// <summary>
        /// On Awake we grab our weapon
        /// </summary>
        protected virtual void Awake()
        {
            _weapon = this.GetComponent<Weapon>();
            _shootStopped = false;
        }

        /// <summary>
        /// On Update, we check for obstacles and prevent shooting if needed
        /// </summary>
        protected virtual void Update()
        {
            if (_weapon == null)
            {
                return;
            }
            if (_weapon.Owner == null)
            {
                return;
            }
            if (_characterHandleWeapon == null)
            {
                _characterHandleWeapon = _weapon.Owner.GetComponent<CharacterHandleWeapon>();
            }
            if (_characterHandleWeapon == null)
            {
                return;
            }

            // if we've found an obstacle, we prevent shooting
            if (CheckForObstacles())
            {
                _characterHandleWeapon.ShootStop();
                _characterHandleWeapon.AbilityPermitted = false;
                _shootStopped = true;
            }
            else
            {
                // if we haven't found an obstacle and were preventing shoot, we resume
                if (_shootStopped)
                {
                    _shootStopped = false;
                    _characterHandleWeapon.AbilityPermitted = true;
                }
            }
            
        }

        /// <summary>
        /// Casts rays in front of the weapon to check for obstacles
        /// Returns true if an obstacle was found
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckForObstacles()
        {
            _hitLeft = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, Quaternion.Euler(0f, -Angle/2f, 0f) * this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);
            _hitMiddle = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);
            _hitRight = MMDebug.Raycast3D(this.transform.position + this.transform.rotation * RaycastOriginOffset, Quaternion.Euler(0f, Angle / 2f, 0f) * this.transform.forward, Distance, ObstacleLayerMask, Color.yellow, true);

            if ((_hitLeft.collider == null) && (_hitMiddle.collider == null) && (_hitRight.collider == null))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
