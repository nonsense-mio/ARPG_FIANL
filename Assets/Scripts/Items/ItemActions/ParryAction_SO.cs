using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "Item Actions/Parry Action")]
    public class ParryAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if(character.isInteracting || character.isUsingComsumable)
                return;
            character.characterAnimatorManager.EraseHandIKForWeapon();
            WeaponItem_SO parryingWeapon = character.characterInventoryManager.currentItemBeingUsed as WeaponItem_SO;

            if(parryingWeapon.weaponType == E_WeaponType.SmallShield)
            {
                character.characterAnimatorManager.PlayTargetAnimation("Parry", true);
            }
            else if(parryingWeapon.weaponType == E_WeaponType.Shield)
            {
                character.characterAnimatorManager.PlayTargetAnimation("Parry", true);
            }
        }
    }

}
