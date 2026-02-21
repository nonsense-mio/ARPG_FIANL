using UnityEngine;


namespace ARPG
{
    [CreateAssetMenu(menuName = "Item Actions/Charge Attack Action")]
    public class ChargeAttackAction_SO : ItemAction_SO
    {
        public override void PerformAction(CharacterManager character)
        {
            if (character.characterStatsManager.currentStamina <= 0 || character.isUsingComsumable)
                return;
            //抹去手部IK
            character.characterAnimatorManager.EraseHandIKForWeapon();
            //播放武器特效
            //character.characterEffectsManager.PlayWeaponFX(false);

            //如果可以连击 就进行连击
            if (character.canDoCombo)
            {
                HandleChargeAttackCombo(character);
                character.canDoCombo = false;
            }
            //如果不能连击 就进行普通攻击
            else
            {
                if (character.isInteracting)
                    return;
                HandleChargeAttack(character);
            }
        }

        private void HandleChargeAttack(CharacterManager character)
        {
            if (character.isUsingLeftHand)
            {

                character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_01, true, false, true);
                character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_01;

            }
            else if (character.isUsingRightHand)
            {
                if (character.isTwoHandingWeapon)
                {
                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_charge_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.th_charge_attack_01;
                }
                else
                {

                    character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_01, true);
                    character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_01;
                }
            }


        }


        /// <summary>
        /// 处理连击的方法
        /// </summary>
        private void HandleChargeAttackCombo(CharacterManager character)
        {
            if (character.canDoCombo)
            {
                character.animator.SetBool("canDoCombo", false);

                if (character.isUsingLeftHand)
                {
                    if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_charge_attack_01)
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_02, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_02;
                    }
                    else
                    {
                        character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_01, true, false, true);
                        character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_01;
                    }

                }
                else if (character.isUsingRightHand)
                {
                    if (character.isTwoHandingWeapon)
                    {
                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.th_charge_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_charge_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_charge_attack_02;
                        }
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.th_charge_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.th_charge_attack_01;
                        }
                    }
                    else
                    {

                        if (character.characterCombatManager.lastAttack == character.characterCombatManager.oh_charge_attack_01)
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_02, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_02;
                        }
                        else
                        {
                            character.characterAnimatorManager.PlayTargetAnimation(character.characterCombatManager.oh_charge_attack_01, true);
                            character.characterCombatManager.lastAttack = character.characterCombatManager.oh_charge_attack_01;
                        }
                    }
                }

            }

        }
    }
}