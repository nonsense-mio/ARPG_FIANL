using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
    [CreateAssetMenu(menuName = "A.I/Humanoid Actions/Item Based Attack Action")]
    public class ItemBasedAttackAction : EnemyAction
    {
        [Header("攻击类型")]
        public E_AIAttackActionType attackActionType = E_AIAttackActionType.MeleeAttackAction;
        public E_AttackType attackType = E_AttackType.LightAttack;

        [Header("动作连击相关")]
        public bool canCombo = false;

        [Header("右手/左手动作")]
        //bool isRightHandedAction = true;

        [Header("动作设置")]
        public int attckScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = 35;
        public float minimumAttackAngle = -35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 3;

        //执行攻击行为
        public void PerformAttackAction(EnemyManager enemy)
        {
            if (isRightHandedAction)
            {
                enemy.UpdateWhichHandCharacterIsUsing(true);
                PerformRightHandAttack(enemy);
            }
            else
            {
                enemy.UpdateWhichHandCharacterIsUsing(false);
                PerformLeftHandAttack(enemy);
            }
        }

        //右手攻击动作
        private void PerformRightHandAttack(EnemyManager enemy)
        {
            if(attackActionType == E_AIAttackActionType.MeleeAttackAction)
            {
                PerformRightHandMeleeAction(enemy);
            }
            else if(attackActionType == E_AIAttackActionType.RangedAttackAction)
            {
                //TODO
            }
            else if(attackActionType == E_AIAttackActionType.MagicAttackAction)
            {
                //TODO
            }
        }
        //左手攻击动作
        private void PerformLeftHandAttack(EnemyManager enemy)
        {
            if (attackActionType == E_AIAttackActionType.MeleeAttackAction)
            {

            }
            else if (attackActionType == E_AIAttackActionType.RangedAttackAction)
            {
                //TODO
            }
            else if (attackActionType == E_AIAttackActionType.MagicAttackAction)
            {
                //TODO
            }
        }

        //执行右手近战动作
        private void PerformRightHandMeleeAction(EnemyManager enemy)
        {
            if(enemy.isTwoHandingWeapon)
            {
                if(attackType == E_AttackType.LightAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.th_tap_Mouse_L_Action.PerformAction(enemy);
                }
                else if(attackType == E_AttackType.HeavyAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.th_tap_Mouse_R_Action.PerformAction(enemy);
                }
            }
            else
            {
                if (attackType == E_AttackType.LightAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.oh_tap_Mouse_L_Action.PerformAction(enemy);
                }
                else if (attackType == E_AttackType.HeavyAttack)
                {
                    enemy.characterInventoryManager.rightWeapon.oh_tap_Mouse_R_Action.PerformAction(enemy);
                }
            }
        }
    }

}
