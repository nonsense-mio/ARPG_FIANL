using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
namespace ARPG
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        public void Init(PlayerManager playerMgr)
        {
            player = playerMgr;
        }

        //根据攻击类型消耗体力
        public override void DrainStaminaBasedOnAttackType()
        {
            if (player.isUsingRightHand)
            {
                if (currentAttackType == E_AttackType.LightAttack)
                {
                    player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStamina * player.playerInventoryManager.rightWeapon.lightAttackStaminaMultiplier);
                }
                else if (currentAttackType == E_AttackType.HeavyAttack)
                {
                    player.playerStatsManager.DeductStamina(player.playerInventoryManager.rightWeapon.baseStamina * player.playerInventoryManager.rightWeapon.heavyAttackStaminaMultiplier);
                }
            }
            else if (player.isUsingLeftHand)
            {
                if (currentAttackType == E_AttackType.LightAttack)
                {
                    player.playerStatsManager.DeductStamina(player.playerInventoryManager.leftWeapon.baseStamina * player.playerInventoryManager.leftWeapon.lightAttackStaminaMultiplier);
                }
                else if (currentAttackType == E_AttackType.HeavyAttack)
                {
                    player.playerStatsManager.DeductStamina(player.playerInventoryManager.leftWeapon.baseStamina * player.playerInventoryManager.leftWeapon.heavyAttackStaminaMultiplier);
                }
            }
        }


        public override void AttemptBlock(DamageCollider attackingWeapon, float physicalDamage, float fireDamage, string blockAnimation)
        {
            base.AttemptBlock(attackingWeapon, physicalDamage, fireDamage, blockAnimation);
            player.playerStatsManager.DeductStamina(0); //更新体力UI
        }
    }
}

