using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{	
	[CreateAssetMenu(fileName = "InventoryWeapon", menuName = "MoreMountains/TopDownEngine/InventoryWeapon", order = 2)]
	[Serializable]
    /// <summary>
    /// Weapon item in the TopDown Engine
    /// </summary>
    public class InventoryWeapon : InventoryItem 
	{
        /// the possible auto equip modes
        public enum AutoEquipModes { NoAutoEquip, AutoEquip, AutoEquipIfEmptyHanded }
        
        [Header("Weapon")]
		[Information("Here you need to bind the weapon you want to equip when picking that item.",InformationAttribute.InformationType.Info,false)]
        /// the weapon to equip
        public Weapon EquippableWeapon;
        /// how to equip this weapon when picked : not equip it, automatically equip it, or only equip it if no weapon is currently equipped
        public AutoEquipModes AutoEquipMode = AutoEquipModes.NoAutoEquip;

		/// <summary>
		/// When we grab the weapon, we equip it
		/// </summary>
		public override bool Equip()
		{
			EquipWeapon (EquippableWeapon);
            return true;
		}

		/// <summary>
		/// When dropping or unequipping the weapon, we remove it
		/// </summary>
		public override bool UnEquip()
		{
			EquipWeapon (null);
            return true;
		}

		/// <summary>
		/// Grabs the CharacterHandleWeapon component and sets the weapon
		/// </summary>
		/// <param name="newWeapon">New weapon.</param>
		protected virtual void EquipWeapon(Weapon newWeapon)
		{
			if (EquippableWeapon == null)
			{
				return;
			}
			if (TargetInventory.Owner == null)
			{
				return;
			}
			CharacterHandleWeapon characterHandleWeapon = TargetInventory.Owner.GetComponent<CharacterHandleWeapon>();
			if (characterHandleWeapon != null)
            {
                characterHandleWeapon.ChangeWeapon(newWeapon, this.ItemID);
            }
		}
	}
}
