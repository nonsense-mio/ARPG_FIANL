using Framework;
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
            WeaponItem_SO weapon = player.isUsingRightHand
                ? player.playerInventoryManager.rightWeapon
                : player.playerInventoryManager.leftWeapon;
            if (weapon == null) return;

            float cost = this.SendQuery(new GetAttackStaminaCostQuery(
                weapon.baseStamina, weapon.lightAttackStaminaMultiplier,
                weapon.heavyAttackStaminaMultiplier, currentAttackType));
            player.playerStatsManager.DeductStamina(cost);
        }


        public override void AttemptBlock(DamageCollider attackingWeapon, float physicalDamage, float fireDamage, string blockAnimation)
        {
            base.AttemptBlock(attackingWeapon, physicalDamage, fireDamage, blockAnimation);
            player.playerStatsManager.DeductStamina(0); //更新体力UI
        }
    }
}

