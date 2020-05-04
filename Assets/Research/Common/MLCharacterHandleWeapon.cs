using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

[AddComponentMenu("TopDown Engine/Character/Abilities/ML Character Handle Weapon")]
public class MLCharacterHandleWeapon : CharacterHandleWeapon
{
    protected override void OnDeath()
    {
        var previousWeapon = CurrentWeapon;
        CurrentWeapon = null;
        base.OnDeath();
        CurrentWeapon = previousWeapon;
    }
}
