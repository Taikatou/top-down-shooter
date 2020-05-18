using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// Add this class to a collectible to have the player change weapon when collecting it
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Pickable Weapon")]
	public class PickableWeapon : PickableItem
	{
		/// the new weapon the player gets when collecting this object
		public Weapon WeaponToGive;

        protected CharacterHandleWeapon _handleWeapon;


		/// <summary>
		/// What happens when the weapon gets picked
		/// </summary>
		protected override void Pick(GameObject picker)
		{
            _handleWeapon = _collidingObject.gameObject.MMGetComponentNoAlloc<CharacterHandleWeapon>();
			if (_handleWeapon != null)
			{
				if (_handleWeapon.CanPickupWeapons)
                {
                    _handleWeapon.ChangeWeapon(WeaponToGive, null);
                }
			}	
		}

		/// <summary>
		/// Checks if the object is pickable.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		protected override bool CheckIfPickable()
		{
			_character = _collidingObject.GetComponent<Character>();

			// if what's colliding with the coin ain't a characterBehavior, we do nothing and exit
			if ((_character == null) || (_collidingObject.GetComponent<CharacterHandleWeapon>() == null))
			{
				return false;
			}
			if (_character.CharacterType != Character.CharacterTypes.Player)
			{
				return false;
			}
			return true;
		}
	}
}