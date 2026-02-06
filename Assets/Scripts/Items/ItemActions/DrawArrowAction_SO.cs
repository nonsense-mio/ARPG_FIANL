using System.Collections;
using System.Collections.Generic;
using ARPG;
using UnityEngine;
namespace HT
{
    [CreateAssetMenu(menuName = "Item Actions/Draw Arrow Action")]
    public class DrawArrowAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if(character.isInteracting || character.isUsingComsumable)
                return;
            if(character.isHoldingArrow)
                return;
            character.animator.SetBool("isHoldingArrow", true);
            character.characterAnimatorManager.PlayTargetAnimation("Bow_TH_Draw_01", true);

            //GameObject loadedArrow = Instantiate(player.playerInventoryManager.currentAmmo.loadedItemModel,player.playerWeaponSlotManager.leftHandSlot.transform);
            GameObject loadedArrow = GameArchitecture.Interface.GetSystem<PoolSystem>().Spawn(character.characterInventoryManager.currentAmmo.loadedItemModelName);
            loadedArrow.transform.parent = character.characterWeaponSlotManager.leftHandSlot.parentOverride;
            loadedArrow.transform.localPosition = Vector3.zero;
            loadedArrow.transform.localScale = Vector3.one;
            loadedArrow.transform.localRotation = Quaternion.Euler(Vector3.zero);
            character.characterEffectsManager.instantiateFX = loadedArrow;

            //拉弓
            Animator bowAnimator = character.characterWeaponSlotManager.rightHandSlot.GetComponentInChildren<Animator>();
            bowAnimator.SetBool("isDrawn", true);
            bowAnimator.Play("Bow_TH_Draw_01");
        }
    }
}

