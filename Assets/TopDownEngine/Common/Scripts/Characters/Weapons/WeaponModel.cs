using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// A class used to force a model to aim at a Weapon's target
    /// </summary>
	[AddComponentMenu("TopDown Engine/Weapons/Weapon Model")]
    public class WeaponModel : MonoBehaviour
    {
        /// if this is true, the model will aim at the parent weapon's target
        public bool AimWeaponModelAtTarget = true;

        protected CharacterHandleWeapon _handleWeapon;
        protected WeaponAim _weaponAim;

        /// <summary>
        /// On Start we grab our CharacterHandleWeapon component
        /// </summary>
        protected virtual void Start()
        {
            _handleWeapon = this.GetComponentInParent(typeof(CharacterHandleWeapon)) as CharacterHandleWeapon;
        }

        /// <summary>
        /// Aims the weapon model at the target
        /// </summary>
        protected virtual void Update()
        {
            if (!AimWeaponModelAtTarget)
            {
                return;
            }

            if (_weaponAim == null)
            {
                _weaponAim = _handleWeapon.CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
            }
            else
            {
                this.transform.LookAt(_weaponAim.transform.position + 10f * _weaponAim.CurrentAim);
            }
        }
    }
}
