using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG
{
    public class AttackStateHumanoid : State
    {
        RotateTowardsTargetStateHumanoid rotateTowardsTargetState;
        CombatStanceStateHumanoid combatStanceState;
        PursueTargetStateHumanoid pursueTargetState;
        PatrolStateHumanoid patrolState;
        public ItemBasedAttackAction currentAttack;

        bool willDoComboOnNext = false;
        public bool hasperformedAttack = false;

        private void Awake()
        {
            rotateTowardsTargetState = GetComponent<RotateTowardsTargetStateHumanoid>();
            combatStanceState = GetComponent<CombatStanceStateHumanoid>();
            pursueTargetState = GetComponent<PursueTargetStateHumanoid>();
            patrolState = GetComponent<PatrolStateHumanoid>();

        }
        //选择一种攻击
        //如果攻击可行 选择一个新攻击
        //如果攻击不可行 停止攻击并攻击目标
        //设置攻击恢复时间
        //返回到战斗姿态的状态
        public override State Tick(EnemyManager enemy)
        {
            if (enemy.currentTarget == null)
            {
                return patrolState;
            }
            if (enemy.combatStyle == E_AICombatStyle.SwordAndShield)
            {
                return ProcessSwordAndShieldCombat(enemy);
            }
            else if (enemy.combatStyle == E_AICombatStyle.Archer)
            {
                return ProcessArcherCombat(enemy);
            }
            else
            {
                return this;
            }

        }


        #region 剑盾攻击方式
        private State ProcessSwordAndShieldCombat(EnemyManager enemy)
        {
            // 1) 目标可能在攻击后死亡/被清空（非常常见）
            if (enemy == null || enemy.currentTarget == null || enemy.currentTarget.isDead)
            {
                ResetStateFlags();
                if (enemy != null) enemy.currentTarget = null;
                return combatStanceState;
            }

            RotateTowardsTargetWhileAttacking(enemy);

            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }

            // 2) currentAttack 为空时，不能继续在 Attack 状态里解引用
            if (currentAttack == null)
            {
                ResetStateFlags();
                return combatStanceState;
            }

            if (willDoComboOnNext && currentAttack.canCombo)
            {
                AttackTargetWithCombo(enemy);
                return this;
            }

            if (!hasperformedAttack && enemy.distanceFromTarget <= currentAttack.maximumDistanceNeededToAttack)
            {
                AttackTarget(enemy);
                RollForComboChance(enemy);
                if (!willDoComboOnNext)
                {
                    currentAttack = null;
                }
            }

            if (willDoComboOnNext && hasperformedAttack)
            {
                return this;
            }

            if (currentAttack != null && enemy.distanceFromTarget > currentAttack.maximumDistanceNeededToAttack)
            {
                currentAttack = null;
            }

            ResetStateFlags();
            return rotateTowardsTargetState;
        }
        #endregion
        #region 弓箭手攻击方式
        private State ProcessArcherCombat(EnemyManager enemy)
        {
            RotateTowardsTargetWhileAttacking(enemy);
            if (enemy.isInteracting)
            {
                return this;
            }
            if (!enemy.isHoldingArrow)
            {
                ResetStateFlags();
                return combatStanceState;
            }
            if (enemy.currentTarget != null && enemy.currentTarget.isDead)
            {
                ResetStateFlags();
                enemy.currentTarget = null;
                return this;
            }

            if (!hasperformedAttack && enemy.isHoldingArrow)
            {
                //射箭
                FireAmmo(enemy);
            }

            //当与目标距离大于仇恨距离时 返回追击状态
            if (enemy.distanceFromTarget > enemy.maximumAggroRadius)
            {
                ResetStateFlags();
                return pursueTargetState;
            }


            ResetStateFlags();
            return rotateTowardsTargetState;
        }
        #endregion

        private void AttackTarget(EnemyManager enemy)
        {
            currentAttack.PerformAttackAction(enemy);
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            hasperformedAttack = true;
        }
        private void AttackTargetWithCombo(EnemyManager enemy)
        {
            currentAttack.PerformAttackAction(enemy);
            willDoComboOnNext = false;
            enemy.currentRecoveryTime = currentAttack.recoveryTime;
            currentAttack = null;
        }

        private void RotateTowardsTargetWhileAttacking(EnemyManager enemy)
        {
            if (enemy == null) return;

            if (enemy.canRotate && enemy.isInteracting)
            {
                if (enemy.currentTarget == null) return; // 防止攻击后目标被清空导致 NRE

                Vector3 direction = enemy.currentTarget.transform.position - enemy.transform.position;
                direction.y = 0;
                direction.Normalize();
                if (direction == Vector3.zero)
                {
                    direction = enemy.transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);

                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
            }

        }

        private void RollForComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);
            if (enemyManager.allowAIToPerformCombos && comboChance <= enemyManager.comboLikelyHood)
            {
                if (currentAttack.canCombo)
                {
                    willDoComboOnNext = true;
                }
                else
                {
                    willDoComboOnNext = false;
                    currentAttack = null;
                }

            }
        }

        private void ResetStateFlags()
        {
            willDoComboOnNext = false;
            hasperformedAttack = false;
        }

        private void FireAmmo(EnemyManager enemy)
        {
            if (enemy.isHoldingArrow)
            {
                hasperformedAttack = true;
                enemy.characterInventoryManager.currentItemBeingUsed = enemy.characterInventoryManager.rightWeapon;
                enemy.characterInventoryManager.rightWeapon.th_tap_Mouse_L_Action.PerformAction(enemy);
            }
        }

    }
}
