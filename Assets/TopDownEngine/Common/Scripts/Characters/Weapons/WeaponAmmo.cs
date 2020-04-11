using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.TopDownEngine
{	
	[RequireComponent(typeof(Weapon))]
    [AddComponentMenu("TopDown Engine/Weapons/Weapon Ammo")]
    public class WeaponAmmo : MonoBehaviour, MMEventListener<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>, MMEventListener<MMInventoryEvent>
	{
		[Header("Ammo")]
		public string AmmoID;
		/// the name of the inventory where the system should look for ammo
		public string AmmoInventoryName = "MainInventory";
		/// the theoretical maximum of ammo
		public int MaxAmmo = 100;
		/// if this is true, everytime you equip this weapon, it'll auto fill with ammo
		public bool ShouldLoadOnStart = true;

		[MMReadOnly]
		/// the current amount of ammo available in the inventory
		public int CurrentAmmoAvailable;
        /// the inventory where ammo for this weapon is stored
		public Inventory AmmoInventory { get; set; }

		protected Weapon _weapon;

        /// <summary>
        /// On start, we grab the ammo inventory if we can find it
        /// </summary>
		protected virtual void Start()
		{
			GameObject ammoInventoryTmp = GameObject.Find (AmmoInventoryName);
			if (ammoInventoryTmp != null) { AmmoInventory = ammoInventoryTmp.GetComponent<Inventory> (); }
			_weapon = GetComponent<Weapon> ();
			LoadOnStart ();
		}

        /// <summary>
        /// Loads our weapon with ammo
        /// </summary>
		protected virtual void LoadOnStart()
		{
			FillWeaponWithAmmo ();
		}

        /// <summary>
        /// Updates the CurrentAmmoAvailable counter
        /// </summary>
		protected virtual void RefreshCurrentAmmoAvailable()
		{
			CurrentAmmoAvailable = AmmoInventory.GetQuantity (AmmoID);
		}

        /// <summary>
        /// Returns true if this weapon has enough ammo to fire, false otherwise
        /// </summary>
        /// <returns></returns>
		public bool EnoughAmmoToFire()
		{
			if (AmmoInventory == null)
			{
				Debug.LogWarning (this.name + " couldn't find the associated inventory. Is there one present in the scene? It should be named '" + AmmoInventoryName + "'.");
				return false;
			}

			RefreshCurrentAmmoAvailable ();

			if (_weapon.MagazineBased)
			{
				if (_weapon.CurrentAmmoLoaded >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (CurrentAmmoAvailable >= _weapon.AmmoConsumedPerShot)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Consumes ammo based on the amount of ammo to consume per shot
        /// </summary>
		protected virtual void ConsumeAmmo()
		{
			if (_weapon.MagazineBased)
			{
				_weapon.CurrentAmmoLoaded = _weapon.CurrentAmmoLoaded - _weapon.AmmoConsumedPerShot;
			}


			for (int i = 0; i < _weapon.AmmoConsumedPerShot; i++)
			{
				AmmoInventory.UseItem (AmmoID);	
				CurrentAmmoAvailable--;
			}

            if (CurrentAmmoAvailable < _weapon.AmmoConsumedPerShot)
            {
                if (_weapon.AutoDestroyWhenEmpty)
                {
                    StartCoroutine(_weapon.WeaponDestruction());
                }

            }
        }

        /// <summary>
        /// Fills the weapon with ammo
        /// </summary>
		public virtual void FillWeaponWithAmmo()
		{
			if (AmmoInventory != null)
			{
				RefreshCurrentAmmoAvailable ();
			}

			if (_weapon.MagazineBased)
			{
				int counter = 0;
				int stock = CurrentAmmoAvailable;
				for (int i = _weapon.CurrentAmmoLoaded; i < _weapon.MagazineSize; i++)
				{
					if (stock > 0)
					{
						stock--;
						counter++;				
					}									
				}
				_weapon.CurrentAmmoLoaded += counter;
			}
		}

        /// <summary>
        /// When getting weapon events, we either consume ammo or refill it
        /// </summary>
        /// <param name="weaponEvent"></param>
		public virtual void OnMMEvent(MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates> weaponEvent)
		{
			// if this event doesn't concern us, we do nothing and exit
			if (weaponEvent.Target != this.gameObject)
			{
				return;
			}

			switch (weaponEvent.NewState)
			{
				case MoreMountains.TopDownEngine.Weapon.WeaponStates.WeaponUse:
					ConsumeAmmo ();
					break;

				case MoreMountains.TopDownEngine.Weapon.WeaponStates.WeaponReloadStop:
					FillWeaponWithAmmo();
					break;
			}
		}

        /// <summary>
        /// Grabs inventory events and refreshes ammo if needed
        /// </summary>
        /// <param name="inventoryEvent"></param>
		public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
		{
			switch (inventoryEvent.InventoryEventType)
			{
				case MMInventoryEventType.Pick:
					if (inventoryEvent.EventItem.ItemClass == ItemClasses.Ammo)
					{
						RefreshCurrentAmmoAvailable ();
					}
					break;				
			}
		}
        
		/// <summary>
		/// On enable, we start listening for MMGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>();
			this.MMEventStartListening<MMInventoryEvent> ();
		}

		/// <summary>
		/// On disable, we stop listening for MMGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMStateChangeEvent<MoreMountains.TopDownEngine.Weapon.WeaponStates>>();
			this.MMEventStopListening<MMInventoryEvent> ();
		}
	}
}