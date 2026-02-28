
using UnityEngine;

namespace ARPG
{
    [CreateAssetMenu(menuName = "Item Actions/Heavy Attack Action")]
    public class HeavyAttackAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if(character.isUsingComsumable)
                return;
            PlayerManager player = character as PlayerManager;
            //只有玩家角色沒有耐力时才阻止攻击
            if (player != null && player.characterStatsManager.currentStamina <= 0)
                return;
            //抹去手部IK
            character.characterAnimatorManager.EraseHandIKForWeapon();
            //播放武器特效
            //character.characterEffectsManager.PlayWeaponFX(false);
            character.isAttacking = true;
            //如果在冲刺 则进行冲刺攻击
            if (character.isSprinting)
            {
                HandleJumpingAttack(character);
                return;
            }
            //如果可以连击 就进行连击
            if (character.canDoCombo)
            {
                HandleHeavyWeaponCombo(character);
            }
            //如果不能连击 就进行普通攻击
            else
            {
                if (character.isInteracting)
                    return;
                HandleHeavyAttack(character);
            }

            //设置当前攻击类型
            character.characterCombatManager.currentAttackType = E_AttackType.HeavyAttack;
        }

        private void HandleHeavyAttack(CharacterManager character)
        {
            if (character.isUsingLeftHand)
            {

                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;

            }
            else if (character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_01;
                }
                else
                {

                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                }
            }


        }


        private void HandleJumpingAttack(CharacterManager character)
        {
            if (character.isUsingLeftHand)
            {
                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_jumping_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_jumping_attack_01;

            }
            else if (character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_jumping_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_jumping_attack_01;
                }
                else
                {

                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_jumping_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_jumping_attack_01;
                }
            }

        }

        /// <summary>
        /// 处理连击的方法
        /// </summary>
        private void HandleHeavyWeaponCombo(CharacterManager character)
        {
            if (character.canDoCombo)
            {
                character.animator.SetBool("canDoCombo", false);

                if (character.isUsingLeftHand)
                {
                    if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_01)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_02, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_02;
                    }
                    else
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                    }

                }
                else if (character.isUsingRightHand)
                {
                    if (character.isTwoHandingWeapon)
                    {
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_heavy_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_02;
                        }
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_heavy_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_heavy_attack_01;
                        }
                    }
                    else
                    {

                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_heavy_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_02;
                        }
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_heavy_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_heavy_attack_01;
                        }
                    }
                }

            }

        }
    }
}

